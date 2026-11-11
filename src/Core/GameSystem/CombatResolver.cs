namespace CliInterface.Core.GameSystem
{
    /// <summary>
    /// Provides pure combat math for the game.
    /// </summary>
    /// <remarks>
    /// Extracted from <see cref="Character"/> (SRP). A static service with no state:
    /// damage formulas and the defend bonus live in exactly one place instead of being
    /// hard-coded inside <c>Character.Attack</c> and <c>Character.UseAbility</c>.
    /// </remarks>
    public static class CombatResolver
    {
        /// <summary>
        /// Gets the temporary armor gained while defending.
        /// </summary>
        /// <value>The temporary armor gained while defending.</value>
        public const int DefendArmorBonus = 5;

        /// <summary>
        /// Gets the default damage multiplier for abilities.
        /// </summary>
        /// <value>The default damage multiplier for abilities.</value>
        public const int DefaultAbilityMultiplier = 2;

        /// <summary>
        /// Calculates the damage of a standard attack.
        /// </summary>
        /// <param name="attackerTotalAttack">The total attack of the attacker.</param>
        /// <param name="targetTotalArmor">The total armor of the target.</param>
        /// <returns>The damage dealt, never below zero.</returns>
        public static int CalculateDamage(int attackerTotalAttack, int targetTotalArmor) =>
            Math.Max(0, attackerTotalAttack - targetTotalArmor);

        /// <summary>
        /// Calculates the damage of an ability.
        /// </summary>
        /// <param name="attackerTotalAttack">The total attack of the ability user.</param>
        /// <param name="multiplier">The damage multiplier of the ability.</param>
        /// <returns>The damage dealt by the ability.</returns>
        public static int CalculateAbilityDamage(int attackerTotalAttack, int multiplier = DefaultAbilityMultiplier) =>
            attackerTotalAttack * multiplier;
    }
}
