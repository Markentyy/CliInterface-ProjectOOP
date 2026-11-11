namespace CliInterface.Cli.Engine
{
    /// <summary>
    /// A tokenized command line: verb, positional arguments and --options.
    /// </summary>
    public sealed class ParsedCommand
    {
        /// <summary>
        /// Gets the command verb in lower case.
        /// </summary>
        public string Verb { get; }

        /// <summary>
        /// Gets the positional arguments in order.
        /// </summary>
        public IReadOnlyList<string> Args { get; }

        /// <summary>
        /// Gets the parsed options by lower case name.
        /// A flag without a value maps to null.
        /// </summary>
        public IReadOnlyDictionary<string, string?> Options { get; }

        /// <summary>
        /// Initializes a parsed command line.
        /// </summary>
        /// <param name="verb">The command verb.</param>
        /// <param name="args">The positional arguments.</param>
        /// <param name="options">The options by name.</param>
        public ParsedCommand(string verb, IReadOnlyList<string> args, IReadOnlyDictionary<string, string?> options)
        {
            Verb = verb;
            Args = args;
            Options = options;
        }

        /// <summary>
        /// Checks whether the given option was provided.
        /// </summary>
        /// <param name="name">The option name without leading dashes.</param>
        /// <returns>True when the option is present.</returns>
        public bool HasOption(string name) => Options.ContainsKey(name);

        /// <summary>
        /// Gets the value of the given option.
        /// </summary>
        /// <param name="name">The option name without leading dashes.</param>
        /// <returns>The option value, or null for flags and missing options.</returns>
        public string? GetOption(string name) =>
            Options.TryGetValue(name, out string? value) ? value : null;

        /// <summary>
        /// Gets the value of a required option or raises a user error.
        /// </summary>
        /// <param name="name">The option name without leading dashes.</param>
        /// <returns>The option value.</returns>
        public string RequireOption(string name)
        {
            string? value = GetOption(name);
            if (string.IsNullOrWhiteSpace(value))
                throw new CommandException($"Missing required option --{name} <value>.");
            return value;
        }

        /// <summary>
        /// Gets the positional argument at the given index or raises a user error.
        /// </summary>
        /// <param name="index">The zero based argument index.</param>
        /// <param name="what">The argument description used in the error text.</param>
        /// <returns>The argument value.</returns>
        public string RequireArg(int index, string what)
        {
            if (index < 0 || index >= Args.Count)
                throw new CommandException(what.EndsWith(".", StringComparison.Ordinal) ? $"Missing {what}" : $"Missing {what}.");
            return Args[index];
        }

        /// <summary>
        /// Checks the positional argument count against the allowed range.
        /// Extra arguments are rejected instead of being silently ignored.
        /// </summary>
        /// <param name="min">The minimum accepted count.</param>
        /// <param name="max">The maximum accepted count.</param>
        /// <param name="usage">The usage line shown in the error text.</param>
        public void ExpectArgCount(int min, int max, string usage)
        {
            if (Args.Count < min || Args.Count > max)
                throw new CommandException($"Wrong number of arguments (got {Args.Count}). Usage: {usage}");
        }

        /// <summary>
        /// Rejects any option outside the allowed list.
        /// </summary>
        /// <param name="usage">The usage line shown in the error text.</param>
        /// <param name="allowed">The allowed option names without leading dashes.</param>
        public void ExpectOptions(string usage, params string[] allowed)
        {
            foreach (string key in Options.Keys)
            {
                if (!allowed.Contains(key, StringComparer.OrdinalIgnoreCase))
                    throw new CommandException($"Unknown option --{key}. Usage: {usage}");
            }
        }
    }
}
