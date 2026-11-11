namespace CliInterface.Core.Catalog
{
    /// <summary>
    /// The characters interpreter state: three registries plus ability books.
    /// Characters learn abilities through the book; items are shared by reference.
    /// </summary>
    public sealed class CharacterWorld
    {
        /// <summary>
        /// Gets the character registry (auto ids "char-N").
        /// </summary>
        public Registry<GameSystem.Character> Characters { get; } =
            new("char", c => c.Name);

        /// <summary>
        /// Gets the item registry (auto ids "item-N").
        /// </summary>
        public Registry<GameSystem.Equipment> Items { get; } =
            new("item", i => i.Name);

        /// <summary>
        /// Gets the ability registry (auto ids "ability-N").
        /// </summary>
        public Registry<GameSystem.Ability> Abilities { get; } =
            new("ability", a => a.Name);

        private readonly Dictionary<string, List<string>> _books =
            new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the ability ids known by the given character id.
        /// </summary>
        /// <param name="characterId">The character id.</param>
        /// <returns>The known ability ids.</returns>
        public IReadOnlyList<string> BookOf(string characterId) =>
            _books.TryGetValue(characterId, out List<string>? book)
                ? book
                : Array.Empty<string>();

        /// <summary>
        /// Attaches an ability to a character book. Already known abilities are ignored.
        /// </summary>
        /// <param name="characterId">The character id.</param>
        /// <param name="abilityId">The ability id.</param>
        /// <returns>True when newly learned, false when already known.</returns>
        public bool Learn(string characterId, string abilityId)
        {
            if (!_books.TryGetValue(characterId, out List<string>? book))
            {
                book = new List<string>();
                _books.Add(characterId, book);
            }
            if (book.Contains(abilityId, StringComparer.OrdinalIgnoreCase))
                return false;
            book.Add(abilityId);
            return true;
        }

        /// <summary>
        /// Removes every stored object and book, resetting the world
        /// before a persistence loader restores saved state.
        /// </summary>
        public void Clear()
        {
            Characters.Clear();
            Items.Clear();
            Abilities.Clear();
            _books.Clear();
        }

        /// <summary>
        /// Resolves a character by id or name.
        /// </summary>
        /// <param name="idOrName">The id or the name.</param>
        /// <returns>The character id and instance.</returns>
        public KeyValuePair<string, GameSystem.Character> ResolveCharacter(string idOrName) =>
            Resolve(Characters, "character", idOrName);

        /// <summary>
        /// Resolves an item by id or name.
        /// </summary>
        /// <param name="idOrName">The id or the name.</param>
        /// <returns>The item id and instance.</returns>
        public KeyValuePair<string, GameSystem.Equipment> ResolveItem(string idOrName) =>
            Resolve(Items, "item", idOrName);

        /// <summary>
        /// Resolves an ability by id or name.
        /// </summary>
        /// <param name="idOrName">The id or the name.</param>
        /// <returns>The ability id and instance.</returns>
        public KeyValuePair<string, GameSystem.Ability> ResolveAbility(string idOrName) =>
            Resolve(Abilities, "ability", idOrName);

        /// <summary>
        /// Finds characters carrying the given item instance.
        /// </summary>
        /// <param name="item">The item instance.</param>
        /// <returns>Character ids and instances using it.</returns>
        public IEnumerable<KeyValuePair<string, GameSystem.Character>> UsersOfItem(GameSystem.Equipment item)
        {
            foreach (KeyValuePair<string, GameSystem.Character> entry in Characters.All)
            {
                if (entry.Value.Inventory.Items.Contains(item))
                    yield return entry;
            }
        }

        /// <summary>
        /// Finds characters knowing the given ability id.
        /// </summary>
        /// <param name="abilityId">The ability id.</param>
        /// <returns>Character ids and instances knowing it.</returns>
        public IEnumerable<KeyValuePair<string, GameSystem.Character>> HoldersOfAbility(string abilityId)
        {
            foreach (KeyValuePair<string, GameSystem.Character> entry in Characters.All)
            {
                if (BookOf(entry.Key).Contains(abilityId, StringComparer.OrdinalIgnoreCase))
                    yield return entry;
            }
        }

        /// <summary>
        /// Resolves one registry entry by id first, then by name.
        /// </summary>
        /// <typeparam name="T">The stored type.</typeparam>
        /// <param name="registry">The registry to search.</param>
        /// <param name="kind">The kind name used in error texts.</param>
        /// <param name="idOrName">The id or the name.</param>
        /// <returns>The entry id and value.</returns>
        private static KeyValuePair<string, T> Resolve<T>(Registry<T> registry, string kind, string idOrName)
            where T : class
        {
            if (registry.ContainsId(idOrName))
                return new KeyValuePair<string, T>(idOrName, registry.GetById(idOrName));

            IReadOnlyList<string> ids = registry.IdsOfName(idOrName);
            if (ids.Count == 1)
                return new KeyValuePair<string, T>(ids[0], registry.GetById(ids[0]));
            if (ids.Count == 0)
                throw new CatalogException($"Unknown {kind} '{idOrName}'.");
            throw new CatalogException(
                $"Ambiguous {kind} name '{idOrName}': {string.Join(", ", ids)}. Use an id.");
        }
    }
}
