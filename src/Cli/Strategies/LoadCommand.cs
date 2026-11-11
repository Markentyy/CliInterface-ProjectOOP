using CliInterface.Cli.Engine;
using CliInterface.Data;
using CliInterface.Core.Catalog;

namespace CliInterface.Cli.Strategies
{
    /// <summary>
    /// Loads the world snapshot from the store, replacing current state.
    /// </summary>
    public sealed class LoadCommand : ICommand
    {
        private readonly CharacterWorld _world;
        private readonly IWorldStore _store;

        /// <summary>
        /// Gets the command verb.
        /// </summary>
        public string Name => "load";

        /// <summary>
        /// Gets the one line help summary.
        /// </summary>
        public string Summary => "Load the catalog from the data file.";

        /// <summary>
        /// Gets the detailed usage line.
        /// </summary>
        public string Usage => "load";

        /// <summary>
        /// Initializes a load command.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <param name="store">The persistence store.</param>
        public LoadCommand(CharacterWorld world, IWorldStore store)
        {
            ArgumentNullException.ThrowIfNull(world);
            ArgumentNullException.ThrowIfNull(store);
            _world = world;
            _store = store;
        }

        /// <summary>
        /// Reads the snapshot and reports the counts.
        /// </summary>
        /// <param name="cli">The CLI facade.</param>
        /// <param name="command">The parsed command line.</param>
        public void Execute(Cli cli, ParsedCommand command)
        {
            command.ExpectArgCount(0, 0, Usage);
            command.ExpectOptions(Usage);
            try
            {
                _store.Load(_world);
            }
            catch (DataException error)
            {
                throw new CommandException(error.Message);
            }
            cli.Console.WriteLine(
                $"Loaded {_world.Characters.Count} characters, " +
                $"{_world.Items.Count} items, {_world.Abilities.Count} abilities " +
                $"from {_store.Location}.");
        }
    }
}
