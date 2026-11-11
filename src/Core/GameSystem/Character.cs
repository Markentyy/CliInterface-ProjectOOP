namespace CliInterface.Core.GameSystem
{
    /// <summary>
    /// Represents a participant in the game world.
    /// </summary>
    /// <remarks>
    /// Slim entity (SRP): holds identity, health and base stats, and delegates
    /// the inventory to <see cref="Inventory"/> and the damage math to <see cref="CombatResolver"/>.
    /// Makes no <c>Console</c> calls - all output is the responsibility of the caller.
    /// Mutable state (<c>Health</c>, <c>IsDefending</c>) can only change through methods.
    /// </remarks>
    public sealed class Character
    {
        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        /// <value>The name of the character.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the current health of the character.
        /// </summary>
        /// <value>The current health of the character.</value>
        public int Health { get; private set; }

        /// <summary>
        /// Gets the maximum health of the character.
        /// </summary>
        /// <value>The maximum health of the character.</value>
        public int MaxHealth { get; }

        /// <summary>
        /// Gets the base armor of the character, without equipment and stance bonuses.
        /// </summary>
        /// <value>The base armor of the character.</value>
        public int BaseArmor { get; }

        /// <summary>
        /// Gets the base attack of the character, without equipment bonuses.
        /// </summary>
        /// <value>The base attack of the character.</value>
        public int BaseAttack { get; }

        /// <summary>
        /// Gets the link to the character artwork.
        /// </summary>
        /// <value>The artwork URL, or null when not provided.</value>
        public string? GraphicsUrl { get; }

        /// <summary>
        /// Gets a value indicating whether the character is in a defensive stance.
        /// </summary>
        /// <value><c>true</c> if the character is defending, otherwise <c>false</c>.</value>
        public bool IsDefending { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the character is defeated.
        /// </summary>
        /// <value><c>true</c> if health dropped to zero, otherwise <c>false</c>.</value>
        public bool IsDefeated => Health <= 0;

        /// <summary>
        /// Gets the inventory owned by the character.
        /// </summary>
        /// <value>The inventory owned by the character.</value>
        /// <remarks>
        /// Composition: the inventory is created with the character and
        /// cannot be replaced from the outside (get-only).
        /// </remarks>
        public Inventory Inventory { get; } = new();

        /// <summary>
        /// Gets the total armor including equipment and stance bonuses.
        /// </summary>
        /// <value>The total armor of the character.</value>
        public int TotalArmor =>
            BaseArmor + Inventory.TotalArmorBonus + (IsDefending ? CombatResolver.DefendArmorBonus : 0);

        /// <summary>
        /// Gets the total attack including equipment bonuses.
        /// </summary>
        /// <value>The total attack of the character.</value>
        public int TotalAttack => BaseAttack + Inventory.TotalAttackBonus;

        /// <summary>
        /// Initializes a new instance of the <see cref="Character"/> class.
        /// </summary>
        /// <param name="name">The name of the character.</param>
        /// <param name="maxHealth">The maximum health of the character.</param>
        /// <param name="baseArmor">The base armor of the character.</param>
        /// <param name="baseAttack">The base attack of the character.</param>
        /// <param name="graphicsUrl">The link to the character artwork, or null.</param>
        public Character(string name, int maxHealth, int baseArmor, int baseAttack, string? graphicsUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Character name must not be empty.", nameof(name));
            if (maxHealth <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxHealth), "Max health must be > 0.");
            if (baseArmor < 0)
                throw new ArgumentOutOfRangeException(nameof(baseArmor), "Base armor must be >= 0.");
            if (baseAttack < 0)
                throw new ArgumentOutOfRangeException(nameof(baseAttack), "Base attack must be >= 0.");

            Name = name;
            MaxHealth = maxHealth;
            Health = maxHealth;
            BaseArmor = baseArmor;
            BaseAttack = baseAttack;
            GraphicsUrl = string.IsNullOrWhiteSpace(graphicsUrl) ? null : graphicsUrl.Trim();
        }

        /// <summary>
        /// Equips an item, boosting the active stats through the inventory.
        /// </summary>
        /// <param name="item">The equipment to equip.</param>
        public void Equip(Equipment item)
        {
            ArgumentNullException.ThrowIfNull(item);
            Inventory.Add(item);
        }

        /// <summary>
        /// Performs a standard attack against a target character.
        /// </summary>
        /// <remarks>
    /// Leaves the defensive stance before striking. Returns the damage dealt
    /// so the caller can report it.
        /// </remarks>
        /// <param name="target">The character to attack.</param>
        /// <returns>The damage dealt to the target.</returns>
        public int Attack(Character target)
        {
            ArgumentNullException.ThrowIfNull(target);
            IsDefending = false;
            int damage = CombatResolver.CalculateDamage(TotalAttack, target.TotalArmor);
            target.TakeDamage(damage);
            return damage;
        }

        /// <summary>
        /// Restores a portion of health without exceeding the maximum capacity.
        /// </summary>
        /// <param name="amount">The amount of health to restore.</param>
        public void Heal(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Heal amount must be >= 0.");
            Health = Math.Min(MaxHealth, Health + amount);
        }

        /// <summary>
        /// Enters a defensive stance, temporarily boosting armor.
        /// </summary>
        /// <remarks>
        /// The stance lasts until the next attack and adds
        /// <see cref="CombatResolver.DefendArmorBonus"/> to the total armor.
        /// </remarks>
        public void Defend() => IsDefending = true;

        /// <summary>
        /// Uses an ability object against a target character.
        /// </summary>
        /// <remarks>
        /// The stringly-typed <c>UseAbility(string, ...)</c> shape is replaced by a real
        /// <see cref="IAbility"/> object (Open/Closed: new abilities mean new classes).
        /// </remarks>
        /// <param name="ability">The ability to use.</param>
        /// <param name="target">The character targeted by the ability.</param>
        /// <returns>The damage dealt to the target.</returns>
        public int UseAbility(IAbility ability, Character target)
        {
            ArgumentNullException.ThrowIfNull(ability);
            ArgumentNullException.ThrowIfNull(target);
            return ability.Use(this, target);
        }

        /// <summary>
        /// Uses an ability identified by name against a target character.
        /// </summary>
        /// <remarks>
        /// Backward-compatible shim for the old <c>UseAbility(string, ...)</c> call shape.
        /// Prefer <see cref="UseAbility(IAbility, Character)"/> with an <see cref="Ability"/> instance.
        /// </remarks>
        /// <param name="abilityName">The name of the ability to use.</param>
        /// <param name="target">The character targeted by the ability.</param>
        /// <returns>The damage dealt to the target.</returns>
        public int UseAbility(string abilityName, Character target) =>
            UseAbility(new Ability(abilityName), target);

        /// <summary>
        /// Applies damage to the character, clamping health at zero.
        /// </summary>
        /// <remarks>
        /// Internal (not private): abilities live in their own class but in the same
        /// assembly, so they can reuse it - while code outside GameSystem still
        /// cannot damage characters directly.
        /// </remarks>
        /// <param name="damage">The amount of damage to apply.</param>
        public void TakeDamage(int damage)
        {
            if (damage < 0)
                throw new ArgumentOutOfRangeException(nameof(damage), "Damage must be >= 0.");
            Health = Math.Max(0, Health - damage);
        }
    }
}
