namespace CliInterface.Cli.Engine
{
    /// <summary>
    /// Error raised for invalid user input: bad syntax, unknown names or missed options.
    /// The REPL loop catches it and prints a friendly message.
    /// </summary>
    public sealed class CommandException : Exception
    {
        /// <summary>
        /// Initializes a new command error with the given message.
        /// </summary>
        /// <param name="message">The user facing error text.</param>
        public CommandException(string message)
            : base(message)
        {
        }
    }
}
