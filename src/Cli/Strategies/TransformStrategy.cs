using CliInterface.Data;

namespace CliInterface.Cli.Strategies
{
    /// <summary>
    /// Transformation strategy: commands that change the catalog
    /// (create objects, give items or abilities).
    /// </summary>
    public sealed class TransformStrategy : CommandStrategy
    {
        /// <summary>
        /// Initializes the strategy and registers its commands.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <param name="store">The persistence store, or null to skip saving.</param>
        public TransformStrategy(CliInterface.Core.Catalog.CharacterWorld world, IWorldStore? store)
            : base("transform")
        {
            Register(new CreateCommand(world, store));
            Register(new AddCommand(world, store));
        }
    }
}
