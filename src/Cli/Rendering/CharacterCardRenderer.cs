using CliInterface.Cli.Engine;
using CliInterface.Core.GameSystem;

namespace CliInterface.Cli.Rendering
{
    /// <summary>
    /// Full data of one character card.
    /// </summary>
    /// <param name="Id">The catalog id.</param>
    /// <param name="Character">The character instance.</param>
    /// <param name="Items">The equipped items with ids.</param>
    /// <param name="AbilityIds">The known ability ids.</param>
    public sealed record CharacterCard(
        string Id,
        Character Character,
        IReadOnlyList<KeyValuePair<string, Equipment>> Items,
        IReadOnlyList<string> AbilityIds);

    /// <summary>
    /// Renderer of the character card: identity, stats with a health bar,
    /// equipment, abilities and the artwork link.
    /// </summary>
    public sealed class CharacterCardRenderer : IRenderer<CharacterCard>
    {
        /// <summary>
        /// Draws the character card.
        /// </summary>
        /// <param name="card">The card data.</param>
        /// <param name="console">The console for output.</param>
        public void Render(CharacterCard card, IConsole console)
        {
            Character c = card.Character;
            console.WriteLine($"[{card.Id}] {c.Name}");
            console.WriteLine($"  Health: {Bar.Of(c.Health, c.MaxHealth)} {c.Health}/{c.MaxHealth}");
            console.WriteLine($"  Attack: {c.BaseAttack} base, {c.TotalAttack} total");
            console.WriteLine($"  Armor:  {c.BaseArmor} base, {c.TotalArmor} total");
            console.WriteLine($"  Graphics: {c.GraphicsUrl ?? "-"}");

            if (card.Items.Count == 0)
            {
                console.WriteLine("  Items: -");
            }
            else
            {
                console.WriteLine("  Items:");
                foreach (KeyValuePair<string, Equipment> item in card.Items)
                {
                    console.WriteLine(
                        $"    [{item.Key}] {item.Value.Name} " +
                        $"(+{item.Value.AttackBonus} ATK, +{item.Value.ArmorBonus} ARM)");
                }
            }

            if (card.AbilityIds.Count == 0)
            {
                console.WriteLine("  Abilities: -");
            }
            else
            {
                console.WriteLine("  Abilities:");
                foreach (string abilityId in card.AbilityIds)
                {
                    console.WriteLine($"    [{abilityId}]");
                }
            }
        }
    }
}
