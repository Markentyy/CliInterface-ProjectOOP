namespace CliInterface.Cli.Strategies
{
    /// <summary>
    /// Information strategy: commands that only read the catalog.
    /// </summary>
    public sealed class InfoStrategy : CommandStrategy
    {
        /// <summary>
        /// Initializes the strategy and registers its commands.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <param name="tableRenderer">The character table renderer.</param>
        /// <param name="cardRenderer">The character card renderer.</param>
        public InfoStrategy(
            CliInterface.Core.Catalog.CharacterWorld world,
            CliInterface.Cli.Rendering.CharacterTableRenderer tableRenderer,
            CliInterface.Cli.Rendering.CharacterCardRenderer cardRenderer)
            : base("info")
        {
            Register(new LsCommand(world, tableRenderer));
            Register(new InfoCommand(world, cardRenderer));
        }
    }
}
