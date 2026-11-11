using CliInterface.Cli.Engine;
namespace CliInterface.Cli.Strategies
{
    /// <summary>
    /// Command bundle of the CLI facade. Each strategy contributes a
    /// group of commands with one responsibility (information,
    /// transformations, infrastructure); the facade merges them into
    /// one dispatch table through <c>UseStrategy</c>.
    /// </summary>
    public interface ICommandStrategy
    {
        /// <summary>
        /// Gets the strategy name used in diagnostics.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the commands contributed by this strategy.
        /// </summary>
        IEnumerable<ICommand> Commands { get; }
    }
}
