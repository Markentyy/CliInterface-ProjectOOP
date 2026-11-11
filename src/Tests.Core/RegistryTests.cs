using CliInterface.Core.Catalog;
using CliInterface.Core.GameSystem;
using Xunit;

namespace Tests.Core
{
    /// <summary>
    /// Registry rules: auto ids, custom ids, duplicates, name lookup
    /// and the loader support (Clear, counter continuation).
    /// </summary>
    public sealed class RegistryTests
    {
        /// <summary>
        /// Builds a registry of named stubs.
        /// </summary>
        /// <returns>The registry.</returns>
        private static Registry<Item> Named() =>
            new("item", i => i.Name);

        /// <summary>
        /// Minimal stored stub for registry tests.
        /// </summary>
        /// <param name="Name">The lookup name.</param>
        public sealed record Item(string Name);

        [Fact]
        public void Add_AutoIds_CountUpFromOne()
        {
            Registry<Item> registry = Named();
            Assert.Equal("item-1", registry.Add(new Item("Sword")));
            Assert.Equal("item-2", registry.Add(new Item("Shield")));
            Assert.Equal(2, registry.Count);
        }

        [Fact]
        public void Add_CustomId_IsUsed()
        {
            Registry<Item> registry = Named();
            Assert.Equal("special", registry.Add(new Item("Sword"), "special"));
            Assert.True(registry.ContainsId("special"));
        }

        [Fact]
        public void Add_DuplicateCustomId_IsRejected()
        {
            Registry<Item> registry = Named();
            registry.Add(new Item("Sword"), "x");
            Assert.Throws<CatalogException>(() => registry.Add(new Item("Shield"), "x"));
        }

        [Fact]
        public void Add_LoadedId_AdvancesAutoCounter()
        {
            Registry<Item> registry = Named();
            registry.Add(new Item("Restored"), "item-7");
            Assert.Equal("item-8", registry.Add(new Item("Fresh")));
        }

        [Fact]
        public void Clear_EmptiesRegistryAndResetsCounter()
        {
            Registry<Item> registry = Named();
            registry.Add(new Item("Sword"));
            registry.Clear();
            Assert.Equal(0, registry.Count);
            Assert.Equal("item-1", registry.Add(new Item("New")));
        }

        [Fact]
        public void IdsOfName_ReturnsAllMatchingIds()
        {
            Registry<Item> registry = Named();
            registry.Add(new Item("Gem"), null);
            registry.Add(new Item("Gem"), null);
            IReadOnlyList<string> ids = registry.IdsOfName("gem");
            Assert.Equal(2, ids.Count);
        }
    }

    /// <summary>
    /// World level resolution rules: id first, name by ambiguity.
    /// </summary>
    public sealed class CharacterWorldTests
    {
        [Fact]
        public void ResolveCharacter_ByAmbiguousName_ReportsAmbiguity()
        {
            CharacterWorld world = new();
            world.Characters.Add(new Character("Twin", 10, 0, 1), null);
            world.Characters.Add(new Character("Twin", 10, 0, 1), null);
            Assert.Throws<CatalogException>(() => world.ResolveCharacter("twin"));
        }

        [Fact]
        public void ResolveCharacter_Unknown_Throws() =>
            Assert.Throws<CatalogException>(() =>
                new CharacterWorld().ResolveCharacter("nobody"));

        [Fact]
        public void Learn_IsIdempotent()
        {
            CharacterWorld world = new();
            world.Abilities.Add(new Ability("Fireball"), null);
            string charId = world.Characters.Add(new Character("Mage", 10, 0, 1), null);
            Assert.True(world.Learn(charId, "ability-1"));
            Assert.False(world.Learn(charId, "ability-1"));
            Assert.Single(world.BookOf(charId));
        }
    }

}
