using CliInterface.Cli.Engine;
using CliInterface.Core.GameSystem;

namespace CliInterface.Cli.Rendering
{
    /// <summary>
    /// One row of the character list view.
    /// </summary>
    /// <param name="Id">The catalog id.</param>
    /// <param name="Character">The character instance.</param>
    /// <param name="ItemCount">The number of equipped items.</param>
    /// <param name="AbilityCount">The number of known abilities.</param>
    public sealed record CharacterRow(string Id, Character Character, int ItemCount, int AbilityCount);

    /// <summary>
    /// Renderer of the character list: one aligned table with a health bar.
    /// </summary>
    public sealed class CharacterTableRenderer : IRenderer<IReadOnlyList<CharacterRow>>
    {
        /// <summary>
        /// Draws the character table.
        /// </summary>
        /// <param name="rows">The rows in listing order.</param>
        /// <param name="console">The console for output.</param>
        public void Render(IReadOnlyList<CharacterRow> rows, IConsole console)
        {
            if (rows.Count == 0)
            {
                console.WriteLine("No characters. Create one with: create char");
                return;
            }

            Table table = new("Id", "Name", "Health", "ATK", "ARM", "Items", "Abilities");
            foreach (CharacterRow row in rows)
            {
                Character c = row.Character;
                table.AddRow(
                    $"[{row.Id}]",
                    c.Name,
                    $"{Bar.Of(c.Health, c.MaxHealth)} {c.Health}/{c.MaxHealth}" +
                        (c.IsDefeated ? " (defeated)" : ""),
                    c.TotalAttack.ToString(),
                    c.TotalArmor.ToString(),
                    row.ItemCount.ToString(),
                    row.AbilityCount.ToString());
            }
            table.Draw(console);
        }
    }
}
