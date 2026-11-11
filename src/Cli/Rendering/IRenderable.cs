using CliInterface.Cli.Engine;

namespace CliInterface.Cli.Rendering
{
    /// <summary>
    /// Renderable view inside the CLI module. Knows its renderer
    /// (aggregated, usually injected through the constructor) and draws
    /// itself on demand. The facade only calls <see cref="Render"/>,
    /// so presentation logic stays with the view that owns the data.
    /// </summary>
    public interface IRenderable
    {
        /// <summary>
        /// Draws the view on the given console using its own renderer.
        /// </summary>
        /// <param name="console">The console for output.</param>
        void Render(IConsole console);
    }
}
