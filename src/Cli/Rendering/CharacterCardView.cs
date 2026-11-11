using CliInterface.Cli.Engine;

namespace CliInterface.Cli.Rendering
{
    /// <summary>
    /// Character card view: <see cref="IRenderable"/> aggregating the
    /// card renderer, same shape as the table view.
    /// </summary>
    public sealed class CharacterCardView : IRenderable
    {
        private readonly CharacterCard _card;
        private readonly IRenderer<CharacterCard> _renderer;

        /// <summary>
        /// Initializes the view over the card data.
        /// </summary>
        /// <param name="card">The card data.</param>
        /// <param name="renderer">The card renderer to aggregate.</param>
        public CharacterCardView(CharacterCard card, IRenderer<CharacterCard> renderer)
        {
            ArgumentNullException.ThrowIfNull(card);
            ArgumentNullException.ThrowIfNull(renderer);
            _card = card;
            _renderer = renderer;
        }

        /// <summary>
        /// Draws the card through the aggregated renderer.
        /// </summary>
        /// <param name="console">The console for output.</param>
        public void Render(IConsole console) => _renderer.Render(_card, console);
    }
}
