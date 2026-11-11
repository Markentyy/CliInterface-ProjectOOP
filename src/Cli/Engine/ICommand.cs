namespace CliInterface.Cli.Engine
{
    /// <summary>
    /// A single terminal command. New commands are added by implementing
    /// this interface and registering the instance inside a strategy.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the command verb used to invoke it.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the one line help summary.
        /// </summary>
        string Summary { get; }

        /// <summary>
        /// Gets the detailed usage line shown by help.
        /// </summary>
        string Usage { get; }

        /// <summary>
        /// Executes the command against the parsed line.
        /// </summary>
        /// <param name="cli">The facade for output and shared services.</param>
        /// <param name="command">The parsed command line.</param>
        void Execute(CliInterface.Cli.Cli cli, ParsedCommand command);
    }
}
