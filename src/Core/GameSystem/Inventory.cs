namespace CliInterface.Core.GameSystem
{
    /// <summary>
    /// Owns and manages the equipment carried by a single character.
    /// </summary>
    /// <remarks>
    /// Extracted from <see cref="Character"/> (SRP): the character no longer manipulates
    /// a raw list itself, all inventory rules live here. The internal list is never
    /// exposed for mutation - only as <c>IReadOnlyList</c>.
    /// </remarks>
    public sealed class Inventory
    {
        private readonly List<Equipment> _items = new();

        /// <summary>
        /// Gets a read-only view of the carried items.
        /// </summary>
        /// <value>A read-only view of the carried items.</value>
        /// <remarks>
        /// Cannot be modified from the outside, use <see cref="Add"/> and <see cref="Remove"/> instead.
        /// </remarks>
        public IReadOnlyList<Equipment> Items => _items.AsReadOnly();

        /// <summary>
        /// Gets the number of carried items.
        /// </summary>
        /// <value>The number of carried items.</value>
        public int Count => _items.Count;

        /// <summary>
        /// Gets the summed attack bonus of all carried items.
        /// </summary>
        /// <value>The summed attack bonus of all carried items.</value>
        public int TotalAttackBonus => _items.Sum(i => i.AttackBonus);

        /// <summary>
        /// Gets the summed armor bonus of all carried items.
        /// </summary>
        /// <value>The summed armor bonus of all carried items.</value>
        public int TotalArmorBonus => _items.Sum(i => i.ArmorBonus);

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="item">The equipment to add.</param>
        public void Add(Equipment item)
        {
            ArgumentNullException.ThrowIfNull(item);
            _items.Add(item);
        }

        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="item">The equipment to remove.</param>
        /// <returns><c>true</c> if the item was present and removed, otherwise <c>false</c>.</returns>
        public bool Remove(Equipment item) => _items.Remove(item);
    }
}
