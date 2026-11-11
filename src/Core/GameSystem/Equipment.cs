namespace CliInterface.Core.GameSystem
{
    /// <summary>
    /// Represents an equippable item that modifies a character's stats.
    /// </summary>
    /// <remarks>
    /// Immutable value object: the state is set once through the constructor
    /// and validated, so it cannot be corrupted from the outside.
    /// </remarks>
    public sealed class Equipment
    {
        /// <summary>
        /// Gets the display name of the equipment.
        /// </summary>
        /// <value>The display name of the equipment.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the attack bonus granted by the equipment.
        /// </summary>
        /// <value>The attack bonus granted by the equipment.</value>
        public int AttackBonus { get; }

        /// <summary>
        /// Gets the armor bonus granted by the equipment.
        /// </summary>
        /// <value>The armor bonus granted by the equipment.</value>
        public int ArmorBonus { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Equipment"/> class.
        /// </summary>
        /// <param name="name">The display name of the equipment.</param>
        /// <param name="attackBonus">The attack bonus granted by the equipment.</param>
        /// <param name="armorBonus">The armor bonus granted by the equipment.</param>
        public Equipment(string name, int attackBonus, int armorBonus)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Equipment name must not be empty.", nameof(name));
            if (attackBonus < 0)
                throw new ArgumentOutOfRangeException(nameof(attackBonus), "Attack bonus must be >= 0.");
            if (armorBonus < 0)
                throw new ArgumentOutOfRangeException(nameof(armorBonus), "Armor bonus must be >= 0.");

            Name = name;
            AttackBonus = attackBonus;
            ArmorBonus = armorBonus;
        }
    }
}
