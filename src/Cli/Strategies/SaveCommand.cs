using CliInterface.Cli.Engine;
using CliInterface.Data;
using CliInterface.Core.Catalog;

namespace CliInterface.Cli.Strategies
{
    /// <summary>
    /// Saves the current world snapshot to the store.
    /// </summary>
    public sealed class SaveCommand : ICommand
    {
        private readonly CharacterWorld _world;
        private readonly IWorldStore _store;

        /// <summary>
        /// Gets the command verb.
        /// </summary>
        public string Name => "save";

        /// <summary>
        /// Gets the one line help summary.
        /// </summary>
        public string Summary => "Save the catalog to the data file.";

        /// <summary>
        /// Gets the detailed usage line.
        /// </summary>
        public string Usage => "save";

        /// <summary>
        /// Initializes a save command.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <param name="store">The persistence store.</param>
        public SaveCommand(CharacterWorld world, IWorldStore store)
        {
            ArgumentNullException.ThrowIfNull(world);
            ArgumentNullException.ThrowIfNull(store);
            _world = world;
            _store = store;
        }

        /// <summary>
        /// Writes the snapshot and reports the location.
        /// </summary>
        /// <param name="cli">The CLI facade.</param>
        /// <param name="command">The parsed command line.</param>
        public void Execute(Cli cli, ParsedCommand command)
        {
            command.ExpectArgCount(0, 0, Usage);
            command.ExpectOptions(Usage);
            try
            {
                _store.Save(_world);
            }
            catch (DataException error)
            {
                throw new CommandException(error.Message);
            }
            cli.Console.WriteLine($"Saved to {_store.Location}.");
        }
    }
}
