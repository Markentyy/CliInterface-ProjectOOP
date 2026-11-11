using CliInterface.Cli.Engine;

namespace CliInterface.Cli
{
    /// <summary>
    /// Interactive read-eval loop over the CLI facade: prints the banner,
    /// reads lines behind the prompt and hands them to the facade.
    /// </summary>
    public static class Repl
    {
        /// <summary>
        /// Runs the loop until exit is typed or the input ends.
        /// </summary>
        /// <param name="console">The console for input and output.</param>
        /// <param name="cli">The facade with registered strategies.</param>
        /// <param name="intro">The banner printed once before the first prompt.</param>
        public static void Run(IConsole console, Cli cli, string intro)
        {
            ArgumentNullException.ThrowIfNull(console);
            ArgumentNullException.ThrowIfNull(cli);
            console.WriteLine(intro);

            while (!cli.QuitRequested)
            {
                console.Write("> ");
                string? line = console.ReadLine();
                if (line is null)
                    return;
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                cli.ExecCommand(line);
            }
        }
    }
}
