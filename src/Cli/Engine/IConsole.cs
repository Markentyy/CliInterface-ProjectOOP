namespace CliInterface.Cli.Engine
{
    /// <summary>
    /// Console abstraction used by commands and the REPL loop.
    /// Keeps command code testable and free of direct Console calls.
    /// </summary>
    public interface IConsole
    {
        /// <summary>
        /// Writes a message without a trailing new line.
        /// </summary>
        /// <param name="message">The message to write.</param>
        void Write(string? message);

        /// <summary>
        /// Writes a message followed by a new line.
        /// </summary>
        /// <param name="message">The message to write, or null for an empty line.</param>
        void WriteLine(string? message = null);

        /// <summary>
        /// Reads one input line, or null on end of input.
        /// </summary>
        /// <returns>The entered line, or null on end of input.</returns>
        string? ReadLine();
    }
}
