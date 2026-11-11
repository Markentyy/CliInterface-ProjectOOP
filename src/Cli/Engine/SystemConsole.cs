namespace CliInterface.Cli.Engine
{
    /// <summary>
    /// Default console implementation backed by the system console.
    /// </summary>
    public sealed class SystemConsole : IConsole
    {
        /// <summary>
        /// Writes a message to the console without a trailing new line.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public void Write(string? message) => Console.Write(message);

        /// <summary>
        /// Writes a message to the console followed by a new line.
        /// </summary>
        /// <param name="message">The message to write, or null for an empty line.</param>
        public void WriteLine(string? message = null) => Console.WriteLine(message);

        /// <summary>
        /// Reads one line from the console, or null on end of input.
        /// </summary>
        /// <returns>The entered line, or null on end of input.</returns>
        public string? ReadLine() => Console.ReadLine();
    }
}
