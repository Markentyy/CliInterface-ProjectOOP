namespace CliInterface.Data
{
    /// <summary>
    /// Persistence contract for the character catalog: full world snapshot
    /// save and load. Presentation code never touches files directly.
    /// </summary>
    public interface IWorldStore
    {
        /// <summary>
        /// Gets the human readable location this store persists to.
        /// </summary>
        string Location { get; }

        /// <summary>
        /// Saves the world snapshot.
        /// </summary>
        /// <param name="world">The character world to save.</param>
        void Save(CliInterface.Core.Catalog.CharacterWorld world);

        /// <summary>
        /// Loads the world snapshot, replacing the current world content.
        /// </summary>
        /// <param name="world">The world to fill from stored data.</param>
        void Load(CliInterface.Core.Catalog.CharacterWorld world);
    }
}
