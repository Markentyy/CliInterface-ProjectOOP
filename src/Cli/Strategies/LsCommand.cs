using CliInterface.Cli.Engine;
using CliInterface.Cli.Rendering;
using CliInterface.Core.Catalog;
using CliInterface.Core.GameSystem;

namespace CliInterface.Cli.Strategies
{
    /// <summary>
    /// Lists stored objects in short tabular form.
    /// </summary>
    public sealed class LsCommand : ICommand
    {
        private readonly CharacterWorld _world;
        private readonly CharacterTableRenderer _renderer;

        /// <summary>
        /// Gets the command verb.
        /// </summary>
        public string Name => "ls";

        /// <summary>
        /// Gets the one line help summary.
        /// </summary>
        public string Summary => "List characters (or items, abilities) in a table.";

        /// <summary>
        /// Gets the detailed usage line.
        /// </summary>
        public string Usage => "ls <chars|items|abilities>";

        /// <summary>
        /// Initializes a list command bound to the given world.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <param name="renderer">The character table renderer.</param>
        public LsCommand(CharacterWorld world, CharacterTableRenderer renderer)
        {
            ArgumentNullException.ThrowIfNull(world);
            ArgumentNullException.ThrowIfNull(renderer);
            _world = world;
            _renderer = renderer;
        }

        /// <summary>
        /// Prints the requested category listing.
        /// </summary>
        /// <param name="cli">The CLI facade for output.</param>
        /// <param name="command">The parsed command line.</param>
        public void Execute(Cli cli, ParsedCommand command)
        {
            command.ExpectArgCount(0, 1, Usage);
            command.ExpectOptions(Usage);
            string category = command.Args.Count == 0
                ? "chars"
                : command.RequireArg(0, "a category: ls <chars|items|abilities>.");

            switch (category.ToLowerInvariant())
            {
                case "chars" or "char":
                    cli.Display(new CharacterTable(_world, _renderer));
                    break;
                case "items" or "item":
                    ListItems(cli.Console);
                    break;
                case "abilities" or "ability":
                    ListAbilities(cli.Console);
                    break;
                default:
                    throw new CommandException(
                        $"Unknown category '{category}'. Use chars, items or abilities.");
            }
        }

        /// <summary>
        /// Prints one line per item with its users.
        /// </summary>
        /// <param name="console">The console for output.</param>
        private void ListItems(IConsole console)
        {
            if (_world.Items.Count == 0)
            {
                console.WriteLine("No items. Create one with: create item");
                return;
            }
            foreach (KeyValuePair<string, Equipment> entry in _world.Items.All)
            {
                Equipment item = entry.Value;
                string users = string.Join(", ",
                    _world.UsersOfItem(item).Select(u => u.Value.Name));
                console.WriteLine($"[{entry.Key}] {item.Name} " +
                    $"(+{item.AttackBonus} ATK, +{item.ArmorBonus} ARM) - used by: {Fallback(users)}");
            }
        }

        /// <summary>
        /// Prints one line per ability with its holders.
        /// </summary>
        /// <param name="console">The console for output.</param>
        private void ListAbilities(IConsole console)
        {
            if (_world.Abilities.Count == 0)
            {
                console.WriteLine("No abilities. Create one with: create ability");
                return;
            }
            foreach (KeyValuePair<string, Ability> entry in _world.Abilities.All)
            {
                Ability ability = entry.Value;
                string holders = string.Join(", ",
                    _world.HoldersOfAbility(entry.Key).Select(h => h.Value.Name));
                console.WriteLine($"[{entry.Key}] {ability.Name} (x{ability.DamageMultiplier}) " +
                    $"- known by: {Fallback(holders)}");
            }
        }

        /// <summary>
        /// Formats a possibly empty join result.
        /// </summary>
        /// <param name="joined">The joined names.</param>
        /// <returns>The joined names or a dash.</returns>
        private static string Fallback(string joined) =>
            joined.Length == 0 ? "-" : joined;
    }
}
