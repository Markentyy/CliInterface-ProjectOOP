using System.Text.Json;
using CliInterface.Core.Catalog;
using CliInterface.Core.GameSystem;

namespace CliInterface.Data
{
    /// <summary>
    /// JSON file based <see cref="IWorldStore"/>. The file is rewritten
    /// atomically: data goes to a temp file first, then replaces the target.
    /// Load rebuilds registries with their original ids and fixes the auto
    /// id counters past them.
    /// </summary>
    public sealed class JsonWorldStore : IWorldStore
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
        };

        private readonly string _path;

        /// <summary>
        /// Initializes a store over the given file path.
        /// </summary>
        /// <param name="path">The JSON file path.</param>
        public JsonWorldStore(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Store path must not be empty.", nameof(path));
            _path = path;
        }

        /// <summary>
        /// Gets the human readable storage location.
        /// </summary>
        public string Location => _path;

        /// <summary>
        /// Saves the full world snapshot to the JSON file.
        /// </summary>
        /// <param name="world">The character world to save.</param>
        public void Save(CharacterWorld world)
        {
            ArgumentNullException.ThrowIfNull(world);
            WorldDto dto = Collect(world);

            try
            {
                string? dir = Path.GetDirectoryName(Path.GetFullPath(_path));
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                string temp = _path + ".tmp";
                File.WriteAllText(temp, JsonSerializer.Serialize(dto, Options));
                File.Move(temp, _path, overwrite: true);
            }
            catch (Exception error) when (error is IOException or UnauthorizedAccessException
                or NotSupportedException)
            {
                throw new DataException($"Cannot save to '{_path}': {error.Message}", error);
            }
        }

        /// <summary>
        /// Loads the snapshot from the JSON file, replacing the current content.
        /// </summary>
        /// <param name="world">The world to fill from stored data.</param>
        public void Load(CharacterWorld world)
        {
            ArgumentNullException.ThrowIfNull(world);
            if (!File.Exists(_path))
                throw new DataException($"No saved file '{_path}'.");

            WorldDto dto;
            try
            {
                dto = JsonSerializer.Deserialize<WorldDto>(File.ReadAllText(_path), Options)
                    ?? throw new DataException($"Saved file '{_path}' is empty.");
            }
            catch (JsonException error)
            {
                throw new DataException($"Saved file '{_path}' is malformed: {error.Message}", error);
            }
            catch (IOException error)
            {
                throw new DataException($"Cannot read '{_path}': {error.Message}", error);
            }

            try
            {
                world.Clear();
                Apply(world, dto);
            }
            catch (DataException)
            {
                throw;
            }
            catch (Exception error) when (error is ArgumentException or CatalogException)
            {
                throw new DataException($"Saved file '{_path}' is inconsistent: {error.Message}", error);
            }
        }

        /// <summary>
        /// Maps the live world onto transfer shapes.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <returns>The transfer snapshot.</returns>
        private static WorldDto Collect(CharacterWorld world)
        {
            WorldDto dto = new();
            foreach (KeyValuePair<string, Character> entry in world.Characters.All)
            {
                Character c = entry.Value;
                List<string> itemIds = new();
                foreach (Equipment item in c.Inventory.Items)
                {
                    if (world.Items.TryGetId(item, out string? itemId) && itemId is not null)
                        itemIds.Add(itemId);
                }
                dto.Characters.Add(new CharacterDto
                {
                    Id = entry.Key,
                    Name = c.Name,
                    Health = c.Health,
                    MaxHealth = c.MaxHealth,
                    BaseArmor = c.BaseArmor,
                    BaseAttack = c.BaseAttack,
                    GraphicsUrl = c.GraphicsUrl,
                    ItemIds = itemIds,
                });
            }

            foreach (KeyValuePair<string, Equipment> entry in world.Items.All)
            {
                Equipment i = entry.Value;
                dto.Items.Add(new ItemDto
                {
                    Id = entry.Key,
                    Name = i.Name,
                    AttackBonus = i.AttackBonus,
                    ArmorBonus = i.ArmorBonus,
                });
            }

            foreach (KeyValuePair<string, Ability> entry in world.Abilities.All)
            {
                Ability a = entry.Value;
                dto.Abilities.Add(new AbilityDto
                {
                    Id = entry.Key,
                    Name = a.Name,
                    DamageMultiplier = a.DamageMultiplier,
                });
            }

            foreach (string characterId in world.Characters.All.Select(e => e.Key))
            {
                List<string> book = new(world.BookOf(characterId));
                if (book.Count > 0)
                    dto.Books[characterId] = book;
            }

            return dto;
        }

        /// <summary>
        /// Rebuilds the world from the transfer snapshot.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <param name="dto">The transfer snapshot.</param>
        private static void Apply(CharacterWorld world, WorldDto dto)
        {
            Dictionary<string, Equipment> items = new(StringComparer.OrdinalIgnoreCase);
            foreach (ItemDto item in dto.Items)
            {
                Equipment equipment = new(item.Name, item.AttackBonus, item.ArmorBonus);
                world.Items.Add(equipment, item.Id);
                items[item.Id] = equipment;
            }

            Dictionary<string, Ability> abilities = new(StringComparer.OrdinalIgnoreCase);
            foreach (AbilityDto ability in dto.Abilities)
            {
                Ability a = new(ability.Name, ability.DamageMultiplier);
                world.Abilities.Add(a, ability.Id);
                abilities[ability.Id] = a;
            }

            foreach (CharacterDto c in dto.Characters)
            {
                Character character = new(c.Name, c.MaxHealth, c.BaseArmor, c.BaseAttack, c.GraphicsUrl);
                foreach (string itemId in c.ItemIds)
                {
                    if (!items.TryGetValue(itemId, out Equipment? item))
                        throw new DataException(
                            $"Character '{c.Id}' references unknown item '{itemId}'.");
                    character.Equip(item);
                }
                string assigned = world.Characters.Add(character, c.Id);
                if (c.Health < character.MaxHealth)
                    character.TakeDamage(character.MaxHealth - c.Health);
                if (dto.Books.TryGetValue(c.Id, out List<string>? book))
                {
                    foreach (string abilityId in book)
                    {
                        if (!abilities.ContainsKey(abilityId))
                            throw new DataException(
                                $"Character '{c.Id}' knows unknown ability '{abilityId}'.");
                        world.Learn(c.Id, abilityId);
                    }
                }
            }
        }
    }
}
