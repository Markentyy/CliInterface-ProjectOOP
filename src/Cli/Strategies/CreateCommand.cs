using CliInterface.Cli.Engine;
using CliInterface.Core.Catalog;
using CliInterface.Core.GameSystem;
using CliInterface.Data;

namespace CliInterface.Cli.Strategies
{
    /// <summary>
    /// Creates characters, items and abilities through a question dialog.
    /// Successful mutations are persisted right away, so user data
    /// survives the end of the session.
    /// </summary>
    public sealed class CreateCommand : ICommand
    {
        private readonly CharacterWorld _world;
        private readonly IWorldStore? _store;

        /// <summary>
        /// Gets the command verb.
        /// </summary>
        public string Name => "create";

        /// <summary>
        /// Gets the one line help summary.
        /// </summary>
        public string Summary => "Create a character, item or ability via dialog.";

        /// <summary>
        /// Gets the detailed usage line.
        /// </summary>
        public string Usage => "create <char|item|ability>";

        /// <summary>
        /// Initializes a create command bound to the given world.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <param name="store">The persistence store, or null to skip saving.</param>
        public CreateCommand(CharacterWorld world, IWorldStore? store)
        {
            ArgumentNullException.ThrowIfNull(world);
            _world = world;
            _store = store;
        }

        /// <summary>
        /// Runs the creation dialog for the requested category.
        /// </summary>
        /// <param name="cli">The CLI facade for output.</param>
        /// <param name="command">The parsed command line.</param>
        public void Execute(Cli cli, ParsedCommand command)
        {
            command.ExpectArgCount(1, 1, Usage);
            command.ExpectOptions(Usage);
            string kind = command.RequireArg(0, "a category: create <char|item|ability>.");
            switch (kind.ToLowerInvariant())
            {
                case "char":
                    CreateCharacter(cli);
                    break;
                case "item":
                    CreateItem(cli);
                    break;
                case "ability":
                    CreateAbility(cli);
                    break;
                default:
                    throw new CommandException($"Unknown category '{kind}'. Use char, item or ability.");
            }
        }

        /// <summary>
        /// Runs the character creation dialog.
        /// </summary>
        /// <param name="cli">The CLI facade.</param>
        private void CreateCharacter(Cli cli)
        {
            IConsole console = cli.Console;
            string name = Prompter.ReadRequired(console, "Name");
            int maxHealth = Prompter.ReadInt(console, "MaxHealth", 1, 10000);
            int armor = Prompter.ReadInt(console, "BaseArmor", 0, 1000);
            int attack = Prompter.ReadInt(console, "BaseAttack", 0, 10000);
            string? graphics = Prompter.ReadOptional(console, "Graphics URL (empty to skip)");
            string? id = Prompter.ReadOptional(console, "Id (empty for auto)");

            var character = new Character(name, maxHealth, armor, attack, graphics);
            string assigned = _world.Characters.Add(character, id);
            console.WriteLine(
                $"Created character '{name}' [{assigned}] " +
                $"({maxHealth} HP, {armor} ARM, {attack} ATK).");
            Persist();
        }

        /// <summary>
        /// Runs the item creation dialog.
        /// </summary>
        /// <param name="cli">The CLI facade.</param>
        private void CreateItem(Cli cli)
        {
            IConsole console = cli.Console;
            string name = Prompter.ReadRequired(console, "Name");
            int attack = Prompter.ReadInt(console, "AttackBonus", 0, 10000);
            int armor = Prompter.ReadInt(console, "ArmorBonus", 0, 10000);
            string? id = Prompter.ReadOptional(console, "Id (empty for auto)");

            var item = new Equipment(name, attack, armor);
            string assigned = _world.Items.Add(item, id);
            console.WriteLine($"Created item '{name}' [{assigned}] (+{attack} ATK, +{armor} ARM).");
            Persist();
        }

        /// <summary>
        /// Runs the ability creation dialog.
        /// </summary>
        /// <param name="cli">The CLI facade.</param>
        private void CreateAbility(Cli cli)
        {
            IConsole console = cli.Console;
            string name = Prompter.ReadRequired(console, "Name");
            int multiplier = Prompter.ReadInt(console, "DamageMultiplier", 1, 100);
            string? id = Prompter.ReadOptional(console, "Id (empty for auto, must be free)");

            var ability = new Ability(name, multiplier);
            string assigned = _world.Abilities.Add(ability, id);
            console.WriteLine($"Created ability '{name}' [{assigned}] (x{multiplier}).");
            Persist();
        }

        /// <summary>
        /// Persists the world after a successful mutation.
        /// Persistence errors do not roll back the catalog change;
        /// the message tells the user the data is only in memory.
        /// </summary>
        private void Persist()
        {
            if (_store is null)
                return;
            try
            {
                _store.Save(_world);
            }
            catch (DataException error)
            {
                throw new CommandException(
                    $"{error.Message} The object stays in memory; use save later.");
            }
        }
    }
}
