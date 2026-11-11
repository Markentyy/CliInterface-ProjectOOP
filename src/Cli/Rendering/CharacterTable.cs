using CliInterface.Cli.Engine;
using CliInterface.Core.Catalog;
using CliInterface.Core.GameSystem;

namespace CliInterface.Cli.Rendering
{
    /// <summary>
    /// Character list view: <see cref="IRenderable"/> that aggregates its
    /// <see cref="IRenderer{T}"/> (the variant from the design document
    /// where rendering knowledge travels with the view).
    /// </summary>
    public sealed class CharacterTable : IRenderable
    {
        private readonly IReadOnlyList<CharacterRow> _rows;
        private readonly IRenderer<IReadOnlyList<CharacterRow>> _renderer;

        /// <summary>
        /// Initializes the view over the character catalog.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <param name="renderer">The table renderer to aggregate.</param>
        public CharacterTable(CharacterWorld world, IRenderer<IReadOnlyList<CharacterRow>> renderer)
        {
            ArgumentNullException.ThrowIfNull(world);
            ArgumentNullException.ThrowIfNull(renderer);
            _rows = BuildRows(world);
            _renderer = renderer;
        }

        /// <summary>
        /// Draws the table through the aggregated renderer.
        /// </summary>
        /// <param name="console">The console for output.</param>
        public void Render(IConsole console) => _renderer.Render(_rows, console);

        /// <summary>
        /// Maps the world onto list rows.
        /// </summary>
        /// <param name="world">The character world.</param>
        /// <returns>The rows in listing order.</returns>
        private static IReadOnlyList<CharacterRow> BuildRows(CharacterWorld world)
        {
            List<CharacterRow> rows = new();
            foreach (KeyValuePair<string, Character> entry in world.Characters.All)
            {
                rows.Add(new CharacterRow(
                    entry.Key,
                    entry.Value,
                    entry.Value.Inventory.Count,
                    world.BookOf(entry.Key).Count));
            }
            return rows;
        }
    }
}
