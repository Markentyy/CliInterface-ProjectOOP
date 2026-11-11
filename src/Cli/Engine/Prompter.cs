namespace CliInterface.Cli.Engine
{
    /// <summary>
    /// Small helpers for dialog style input with validation.
    /// Typing "cancel" aborts the whole dialog at any prompt.
    /// </summary>
    public static class Prompter
    {
        /// <summary>
        /// Reads a non empty line, reprompting on empty input.
        /// </summary>
        /// <param name="console">The console for input and output.</param>
        /// <param name="label">The prompt label.</param>
        /// <returns>The entered text.</returns>
        public static string ReadRequired(IConsole console, string label)
        {
            while (true)
            {
                string? input = ReadRaw(console, label);
                if (!string.IsNullOrWhiteSpace(input))
                    return input.Trim();
                console.WriteLine("Value is required.");
            }
        }

        /// <summary>
        /// Reads an optional line. Empty input maps to null.
        /// </summary>
        /// <param name="console">The console for input and output.</param>
        /// <param name="label">The prompt label.</param>
        /// <returns>The entered text, or null when skipped.</returns>
        public static string? ReadOptional(IConsole console, string label)
        {
            string? input = ReadRaw(console, label);
            return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        }

        /// <summary>
        /// Reads an integer within the given range, reprompting on bad input.
        /// </summary>
        /// <param name="console">The console for input and output.</param>
        /// <param name="label">The prompt label.</param>
        /// <param name="min">The minimum accepted value.</param>
        /// <param name="max">The maximum accepted value.</param>
        /// <returns>The entered number.</returns>
        public static int ReadInt(IConsole console, string label, int min, int max)
        {
            while (true)
            {
                string input = ReadRequired(console, $"{label} [{min}-{max}]");
                if (int.TryParse(input, out int value) && value >= min && value <= max)
                    return value;
                console.WriteLine($"Enter a number from {min} to {max}.");
            }
        }

        /// <summary>
        /// Reads a yes/no answer, reprompting on anything else.
        /// </summary>
        /// <param name="console">The console for input and output.</param>
        /// <param name="label">The prompt label.</param>
        /// <returns>True for yes, false for no.</returns>
        public static bool ReadYesNo(IConsole console, string label)
        {
            while (true)
            {
                string input = ReadRequired(console, $"{label} (y/n)");
                if (input.Equals("y", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("yes", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (input.Equals("n", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("no", StringComparison.OrdinalIgnoreCase))
                    return false;
                console.WriteLine("Answer y or n.");
            }
        }

        /// <summary>
        /// Reads one raw line, throwing on cancel or end of input.
        /// </summary>
        /// <param name="console">The console for input and output.</param>
        /// <param name="label">The prompt label.</param>
        /// <returns>The raw entered text.</returns>
        private static string ReadRaw(IConsole console, string label)
        {
            console.Write($"{label}: ");
            string? input = console.ReadLine();
            if (input is null || input.Trim().Equals("cancel", StringComparison.OrdinalIgnoreCase))
                throw new DialogCancelledException();
            return input;
        }
    }
}
