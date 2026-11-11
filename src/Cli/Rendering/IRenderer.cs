using CliInterface.Cli.Engine;

namespace CliInterface.Cli.Rendering
{
    /// <summary>
    /// Renderer contract of the CLI module: transforms one value into
    /// console output. Concrete renderers live next to the data they
    /// draw; nothing outside the module depends on this interface.
    /// </summary>
    /// <typeparam name="T">The rendered value type.</typeparam>
    public interface IRenderer<T>
    {
        /// <summary>
        /// Draws the value on the given console.
        /// </summary>
        /// <param name="item">The value to draw.</param>
        /// <param name="console">The console for output.</param>
        void Render(T item, IConsole console);
    }
}
