namespace CliInterface.Cli.Engine
{
    /// <summary>
    /// Lists all commands known to the CLI facade with their usage lines.
    /// </summary>
    public sealed class HelpCommand : ICommand
    {
        /// <summary>
        /// Gets the command verb.
        /// </summary>
        public string Name => "help";

        /// <summary>
        /// Gets the one line help summary.
        /// </summary>
        public string Summary => "Show available commands grouped by strategy.";

        /// <summary>
        /// Gets the detailed usage line.
        /// </summary>
        public string Usage => "help";

        /// <summary>
        /// Prints every registered command with its usage line.
        /// </summary>
        /// <param name="cli">The CLI facade with the command table.</param>
        /// <param name="command">The parsed command line (ignored).</param>
        public void Execute(Cli cli, ParsedCommand command)
        {
            command.ExpectArgCount(0, 0, Usage);
            command.ExpectOptions(Usage);
            cli.Console.WriteLine("Commands (grouped by strategy):");
            foreach (ICommand cmd in cli.Commands)
            {
                cli.Console.WriteLine($"{cmd.Name} - {cmd.Summary}");
                cli.Console.WriteLine($"  Usage: {cmd.Usage}");
            }
            cli.Console.WriteLine("exit - Leave the interpreter.");
        }
    }
}
