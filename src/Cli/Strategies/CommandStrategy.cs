using CliInterface.Cli.Engine;
namespace CliInterface.Cli.Strategies
{
    /// <summary>
    /// Base class of concrete strategies: owns a private command list
    /// and exposes the registration entry point. Subclasses fill it in
    /// their constructor, so each strategy is a self contained bundle.
    /// </summary>
    public abstract class CommandStrategy : ICommandStrategy
    {
        private readonly List<ICommand> _commands = new();

        /// <summary>
        /// Initializes a strategy with its display name.
        /// </summary>
        /// <param name="name">The strategy name.</param>
        protected CommandStrategy(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Strategy name must not be empty.", nameof(name));
            Name = name;
        }

        /// <summary>
        /// Gets the strategy name used in diagnostics.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the commands contributed by this strategy.
        /// </summary>
        public IEnumerable<ICommand> Commands => _commands;

        /// <summary>
        /// Registers one command in this strategy.
        /// </summary>
        /// <param name="command">The command to register.</param>
        protected void Register(ICommand command)
        {
            ArgumentNullException.ThrowIfNull(command);
            _commands.Add(command);
        }
    }
}
