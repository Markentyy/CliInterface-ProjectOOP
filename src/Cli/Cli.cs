using CliInterface.Cli.Engine;
using CliInterface.Cli.Rendering;
using CliInterface.Cli.Strategies;
using CliInterface.Core.Catalog;

namespace CliInterface.Cli
{
    /// <summary>
    /// Facade of the console interface. External code registers command
    /// strategies through <see cref="UseStrategy"/>, runs raw user input
    /// through <see cref="ExecCommand"/> and draws views through
    /// <see cref="Display{T}"/>. Parsing, dispatching and error mapping
    /// stay behind this single entry point.
    /// </summary>
    public sealed class Cli
    {
        private readonly CommandSet _commands = new();

        /// <summary>
        /// Initializes the facade over the given console.
        /// </summary>
        /// <param name="console">The console for input and output.</param>
        public Cli(IConsole console)
        {
            ArgumentNullException.ThrowIfNull(console);
            Console = console;
        }

        /// <summary>
        /// Gets the console used by every command.
        /// </summary>
        public IConsole Console { get; }

        /// <summary>
        /// Gets all registered commands in name order.
        /// </summary>
        public IEnumerable<ICommand> Commands => _commands.Commands;

        /// <summary>
        /// Gets a value indicating whether an exit command ran.
        /// </summary>
        public bool QuitRequested { get; private set; }

        /// <summary>
        /// Registers the commands of one strategy. Command names must be
        /// unique across strategies.
        /// </summary>
        /// <param name="strategy">The strategy to merge.</param>
        public void UseStrategy(ICommandStrategy strategy)
        {
            ArgumentNullException.ThrowIfNull(strategy);
            foreach (ICommand command in strategy.Commands)
            {
                try
                {
                    _commands.Add(command);
                }
                catch (InvalidOperationException error)
                {
                    throw new CommandException(
                        $"Duplicate command from strategy '{strategy.Name}': {error.Message}");
                }
            }
        }

        /// <summary>
        /// Parses one raw input line and executes the matching command.
        /// User errors are mapped to friendly messages; the method never
        /// throws for normal input mistakes.
        /// </summary>
        /// <param name="raw">The raw input line.</param>
        public void ExecCommand(string raw)
        {
            ArgumentNullException.ThrowIfNull(raw);
            if (string.IsNullOrWhiteSpace(raw))
                return;

            ParsedCommand parsed;
            try
            {
                parsed = LineParser.Parse(raw);
            }
            catch (CommandException error)
            {
                Console.WriteLine($"Error: {error.Message}");
                return;
            }

            if (parsed.Verb == "exit")
            {
                QuitRequested = true;
                return;
            }

            if (!_commands.TryGet(parsed.Verb, out ICommand? cmd) || cmd is null)
            {
                Console.WriteLine($"Unknown command '{parsed.Verb}'. Type 'help'.");
                return;
            }

            try
            {
                cmd.Execute(this, parsed);
            }
            catch (DialogCancelledException)
            {
                Console.WriteLine("Cancelled.");
            }
            catch (Exception error) when (error is CommandException or CatalogException)
            {
                Console.WriteLine($"Error: {error.Message}");
            }
        }

        /// <summary>
        /// Draws a renderable view on the console. The view aggregates
        /// its own renderer; the facade only forwards the console.
        /// </summary>
        /// <typeparam name="T">The concrete view type.</typeparam>
        /// <param name="renderable">The view to draw.</param>
        public void Display<T>(T renderable)
            where T : IRenderable
        {
            ArgumentNullException.ThrowIfNull(renderable);
            renderable.Render(Console);
        }
    }
}
