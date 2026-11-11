namespace CliInterface.Data
{
    /// <summary>
    /// Data transfer shapes written to the store. Plain mutable records
    /// keep the JSON format stable and independent of the domain.
    /// </summary>
    public sealed record WorldDto
    {
        /// <summary>
        /// Gets or sets the stored characters with their ids.
        /// </summary>
        public List<CharacterDto> Characters { get; init; } = new();

        /// <summary>
        /// Gets or sets the stored items with their ids.
        /// </summary>
        public List<ItemDto> Items { get; init; } = new();

        /// <summary>
        /// Gets or sets the stored abilities with their ids.
        /// </summary>
        public List<AbilityDto> Abilities { get; init; } = new();

        /// <summary>
        /// Gets or sets the ability books by character id.
        /// </summary>
        public Dictionary<string, List<string>> Books { get; init; } = new();
    }

    /// <summary>
    /// Gets or sets one stored character.
    /// </summary>
    public sealed record CharacterDto
    {
        /// <summary>
        /// Gets or sets the catalog id.
        /// </summary>
        public string Id { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the stored health at save time.
        /// </summary>
        public int Health { get; init; }

        /// <summary>
        /// Gets or sets the maximum health.
        /// </summary>
        public int MaxHealth { get; init; }

        /// <summary>
        /// Gets or sets the base armor.
        /// </summary>
        public int BaseArmor { get; init; }

        /// <summary>
        /// Gets or sets the base attack.
        /// </summary>
        public int BaseAttack { get; init; }

        /// <summary>
        /// Gets or sets the artwork link, or null.
        /// </summary>
        public string? GraphicsUrl { get; init; }

        /// <summary>
        /// Gets or sets the equipped item ids in order.
        /// </summary>
        public List<string> ItemIds { get; init; } = new();
    }

    /// <summary>
    /// Gets or sets one stored equipment item.
    /// </summary>
    public sealed record ItemDto
    {
        /// <summary>
        /// Gets or sets the catalog id.
        /// </summary>
        public string Id { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the attack bonus.
        /// </summary>
        public int AttackBonus { get; init; }

        /// <summary>
        /// Gets or sets the armor bonus.
        /// </summary>
        public int ArmorBonus { get; init; }
    }

    /// <summary>
    /// Gets or sets one stored ability.
    /// </summary>
    public sealed record AbilityDto
    {
        /// <summary>
        /// Gets or sets the catalog id.
        /// </summary>
        public string Id { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the damage multiplier.
        /// </summary>
        public int DamageMultiplier { get; init; }
    }
}
