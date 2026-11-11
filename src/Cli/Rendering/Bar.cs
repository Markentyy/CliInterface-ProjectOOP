namespace CliInterface.Cli.Rendering
{
    /// <summary>
    /// Console graphic building blocks shared by renderers:
    /// health bars and aligned tables.
    /// </summary>
    public static class Bar
    {
        /// <summary>
        /// Draws a proportional fill bar, for example `[####------]`.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="max">The maximum value (zero clamps to empty).</param>
        /// <param name="width">The bar width in cells.</param>
        /// <returns>The bar string.</returns>
        public static string Of(int current, int max, int width = 10)
        {
            int cells = max <= 0
                ? 0
                : (int)Math.Round((double)current / max * width, MidpointRounding.AwayFromZero);
            cells = Math.Clamp(cells, 0, width);
            return new string('#', cells) + new string('-', width - cells);
        }
    }
}
