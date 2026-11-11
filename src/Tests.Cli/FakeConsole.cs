using CliInterface.Cli.Engine;

namespace Tests.Cli
{
    /// <summary>
    /// In-memory console with scripted input and recorded output.
    /// </summary>
    public sealed class FakeConsole : IConsole
    {
        private readonly Queue<string> _input;

        /// <summary>
        /// Initializes a console with the lines to feed to ReadLine.
        /// </summary>
        /// <param name="input">The scripted input lines in order.</param>
        public FakeConsole(IEnumerable<string> input)
        {
            _input = new Queue<string>(input);
        }

        /// <summary>
        /// Gets everything written to the console so far.
        /// </summary>
        public string Output { get; set; } = string.Empty;

        /// <summary>
        /// Appends text to the output record.
        /// </summary>
        /// <param name="message">The text to write.</param>
        public void Write(string? message) => Output += message ?? string.Empty;

        /// <summary>
        /// Appends text with a new line to the output record.
        /// </summary>
        /// <param name="message">The text to write.</param>
        public void WriteLine(string? message = null) => Output += (message ?? string.Empty) + "\n";

        /// <summary>
        /// Pops the next scripted line, or null when the script ends.
        /// </summary>
        /// <returns>The scripted line, or null on end of input.</returns>
        public string? ReadLine() => _input.Count > 0 ? _input.Dequeue() : null;

        /// <summary>
        /// Checks whether the output contains the given fragment.
        /// </summary>
        /// <param name="fragment">The fragment to find.</param>
        /// <returns>True when present.</returns>
        public bool Contains(string fragment) => Output.Contains(fragment, StringComparison.Ordinal);
    }
}
