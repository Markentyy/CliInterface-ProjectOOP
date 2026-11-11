namespace CliInterface.Cli.Engine
{
    /// <summary>
    /// Named registry of the commands available in one interpreter mode.
    /// </summary>
    public sealed class CommandSet
    {
        private readonly Dictionary<string, ICommand> _commands =
            new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the registered commands ordered by name.
        /// </summary>
        public IEnumerable<ICommand> Commands =>
            _commands.Values.OrderBy(c => c.Name);

        /// <summary>
        /// Registers one command. Names must be unique within the set.
        /// </summary>
        /// <param name="command">The command to register.</param>
        public void Add(ICommand command)
        {
            ArgumentNullException.ThrowIfNull(command);
            if (_commands.ContainsKey(command.Name))
                throw new InvalidOperationException($"Duplicate command '{command.Name}'.");
            _commands.Add(command.Name, command);
        }

        /// <summary>
        /// Finds a command by verb.
        /// </summary>
        /// <param name="name">The command verb.</param>
        /// <param name="command">The found command, or null.</param>
        /// <returns>True when the command exists.</returns>
        public bool TryGet(string name, out ICommand? command) =>
            _commands.TryGetValue(name, out command);
    }
}
