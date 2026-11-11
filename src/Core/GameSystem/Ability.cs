namespace CliInterface.Core.GameSystem
{
    /// <summary>
    /// Defines the contract for special character abilities.
    /// </summary>
    /// <remarks>
    /// Replaces the old stringly-typed <c>UseAbility("Fireball", target)</c> call shape:
    /// each ability is now an object with its own damage rule.
    /// </remarks>
    public interface IAbility
    {
        /// <summary>
        /// Gets the display name of the ability.
        /// </summary>
        /// <value>The display name of the ability.</value>
        string Name { get; }

        /// <summary>
        /// Gets the damage multiplier of the ability.
        /// </summary>
        /// <value>The damage multiplier of the ability.</value>
        int DamageMultiplier { get; }

        /// <summary>
        /// Applies the ability to the target.
        /// </summary>
        /// <param name="user">The character using the ability.</param>
        /// <param name="target">The character targeted by the ability.</param>
        /// <returns>The damage dealt to the target.</returns>
        int Use(Character user, Character target);
    }

    /// <summary>
    /// Represents a damage-dealing ability (e.g. "Fireball").
    /// </summary>
    /// <remarks>
    /// The damage formula is delegated to <see cref="CombatResolver"/>,
    /// so the multiplier is explicit instead of a magic <c>* 2</c>.
    /// </remarks>
    public sealed class Ability : IAbility
    {
        /// <summary>
        /// Gets the display name of the ability.
        /// </summary>
        /// <value>The display name of the ability.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the damage multiplier of the ability.
        /// </summary>
        /// <value>The damage multiplier of the ability.</value>
        public int DamageMultiplier { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ability"/> class.
        /// </summary>
        /// <param name="name">The display name of the ability.</param>
        /// <param name="damageMultiplier">The damage multiplier of the ability.</param>
        public Ability(string name, int damageMultiplier = CombatResolver.DefaultAbilityMultiplier)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ability name must not be empty.", nameof(name));
            if (damageMultiplier < 1)
                throw new ArgumentOutOfRangeException(nameof(damageMultiplier), "Multiplier must be >= 1.");

            Name = name;
            DamageMultiplier = damageMultiplier;
        }

        /// <summary>
        /// Applies the ability to the target.
        /// </summary>
        /// <param name="user">The character using the ability.</param>
        /// <param name="target">The character targeted by the ability.</param>
        /// <returns>The damage dealt to the target.</returns>
        public int Use(Character user, Character target)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(target);
            int damage = CombatResolver.CalculateAbilityDamage(user.TotalAttack, DamageMultiplier);
            target.TakeDamage(damage);
            return damage;
        }
    }
}
