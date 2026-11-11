using CliInterface.Cli.Engine;
using CliInterface.Core.Catalog;
using CliInterface.Core.GameSystem;
using CliInterface.Cli.Rendering;

namespace CliInterface.Cli.Strategies
{
    /// <summary>
    /// Shows the full card of one catalog object, including the
    /// artwork link of a character.
    /// </summary>
    public sealed class InfoCommand : ICommand
    {
        private readonly CharacterWorld _world;
        private readonly CharacterCardRenderer _renderer;

        /// <summary>
        /// Gets the command verb.
        /// </summary>
        public string Name => "info";

        /// <summary>
        /// Gets the one line help summary.
        /// </summary>
        public string Summary => "Show one character (or item, ability) in full detail.";

        /// <summary>
        /// Gets the detailed usage line.
        /// </summary>
        public string Usage => "info <id|name>";

        /// <summary>
        /// Initializes an info command bound to the given world.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <param name="renderer">The character card renderer.</param>
        public InfoCommand(CharacterWorld world, CharacterCardRenderer renderer)
        {
            ArgumentNullException.ThrowIfNull(world);
            ArgumentNullException.ThrowIfNull(renderer);
            _world = world;
            _renderer = renderer;
        }

        /// <summary>
        /// Prints the requested object card.
        /// </summary>
        /// <param name="cli">The CLI facade for output.</param>
        /// <param name="command">The parsed command line.</param>
        public void Execute(Cli cli, ParsedCommand command)
        {
            command.ExpectArgCount(1, 1, Usage);
            command.ExpectOptions(Usage);
            string idOrName = command.RequireArg(0, "an id or name: info <id|name>.");

            if (_world.Characters.ContainsId(idOrName) ||
                _world.Characters.IdsOfName(idOrName).Count > 0)
            {
                KeyValuePair<string, Character> found = _world.ResolveCharacter(idOrName);
                List<KeyValuePair<string, Equipment>> items = new();
                foreach (Equipment item in found.Value.Inventory.Items)
                {
                    _world.Items.TryGetId(item, out string? itemId);
                    items.Add(new KeyValuePair<string, Equipment>(itemId ?? "?", item));
                }
                cli.Display(new CharacterCardView(
                    new CharacterCard(found.Key, found.Value, items, _world.BookOf(found.Key)),
                    _renderer));
                return;
            }

            if (_world.Items.ContainsId(idOrName) || _world.Items.IdsOfName(idOrName).Count > 0)
            {
                ShowItem(cli.Console, _world.ResolveItem(idOrName));
                return;
            }

            if (_world.Abilities.ContainsId(idOrName) || _world.Abilities.IdsOfName(idOrName).Count > 0)
            {
                ShowAbility(cli.Console, _world.ResolveAbility(idOrName));
                return;
            }

            throw new CommandException($"No catalog object '{idOrName}'. Try: ls");
        }

        /// <summary>
        /// Prints the full state of one item.
        /// </summary>
        /// <param name="console">The console for output.</param>
        /// <param name="entry">The item id and instance.</param>
        private static void ShowItem(IConsole console, KeyValuePair<string, Equipment> entry)
        {
            Equipment item = entry.Value;
            console.WriteLine($"[{entry.Key}] {item.Name}");
            console.WriteLine($"  Attack bonus: +{item.AttackBonus}");
            console.WriteLine($"  Armor bonus: +{item.ArmorBonus}");
        }

        /// <summary>
        /// Prints the full state of one ability.
        /// </summary>
        /// <param name="console">The console for output.</param>
        /// <param name="entry">The ability id and instance.</param>
        private static void ShowAbility(IConsole console, KeyValuePair<string, Ability> entry)
        {
            Ability ability = entry.Value;
            console.WriteLine($"[{entry.Key}] {ability.Name}");
            console.WriteLine($"  Damage multiplier: x{ability.DamageMultiplier}");
        }
    }
}
