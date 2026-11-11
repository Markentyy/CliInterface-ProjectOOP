using CliInterface.Data;
using CliInterface.Cli.Engine;
namespace CliInterface.Cli.Strategies
{
    /// <summary>
    /// Infrastructure strategy: persistence and self description
    /// commands that do not change the catalog content itself.
    /// </summary>
    public sealed class InfraStrategy : CommandStrategy
    {
        /// <summary>
        /// Initializes the strategy and registers its commands.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <param name="store">The persistence store.</param>
        public InfraStrategy(CliInterface.Core.Catalog.CharacterWorld world, IWorldStore store)
            : base("infra")
        {
            Register(new SaveCommand(world, store));
            Register(new LoadCommand(world, store));
            Register(new HelpCommand());
        }
    }
}
