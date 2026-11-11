using CliInterface.Core.Catalog;
using CliInterface.Core.GameSystem;
using CliInterface.Data;
using Xunit;

namespace Tests.Data
{
    /// <summary>
    /// JSON store roundtrip and error handling over a temp directory.
    /// </summary>
    public sealed class JsonWorldStoreTests : IDisposable
    {
        private readonly string _dir;

        /// <summary>
        /// Initializes a temp directory per test.
        /// </summary>
        public JsonWorldStoreTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "cli-store-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        /// <summary>
        /// Removes the temp directory.
        /// </summary>
        public void Dispose()
        {
            Directory.Delete(_dir, recursive: true);
        }

        /// <summary>
        /// Builds a path inside the temp directory.
        /// </summary>
        /// <param name="name">The file name.</param>
        /// <returns>The full path.</returns>
        private string PathOf(string name) => System.IO.Path.Combine(_dir, name);

        /// <summary>
        /// Builds a world with one of each object type.
        /// </summary>
        /// <returns>The world.</returns>
        private static CharacterWorld Seeded()
        {
            CharacterWorld world = new();
            Equipment sword = new("Sword", 3, 2);
            Ability fire = new("Fireball", 3);
            world.Items.Add(sword, null);
            world.Abilities.Add(fire, null);
            string charId = world.Characters.Add(
                new Character("Mage", 50, 2, 6, "https://example.com/mage.png"), null);
            world.Characters.GetById(charId).Equip(sword);
            world.Learn(charId, "ability-1");
            Character striker = new("Striker", 10, 0, 10);
            world.Characters.Add(striker, null);
            striker.Attack(world.Characters.GetById(charId));
            return world;
        }

        [Fact]
        public void SaveLoad_Roundtrip_RestoresEverything()
        {
            CharacterWorld source = Seeded();
            CharacterWorld target = new();
            JsonWorldStore store = new(PathOf("world.json"));

            store.Save(source);
            Assert.Equal(PathOf("world.json"), store.Location);
            store.Load(target);

            Assert.Equal(source.Characters.Count, target.Characters.Count);
            Assert.Equal(source.Items.Count, target.Items.Count);
            Assert.Equal(source.Abilities.Count, target.Abilities.Count);

            Character mage = target.Characters.GetById("char-1");
            Assert.Equal("Mage", mage.Name);
            Assert.Equal(44, mage.Health);
            Assert.Equal("https://example.com/mage.png", mage.GraphicsUrl);
            Assert.Single(mage.Inventory.Items);
            Assert.Equal(9, mage.TotalAttack);
            Assert.Single(target.BookOf("char-1"));
        }

        [Fact]
        public void SaveLoad_AutoIdsContinuePastRestored()
        {
            CharacterWorld source = Seeded();
            JsonWorldStore store = new(PathOf("world.json"));
            store.Save(source);
            store.Load(source);

            Assert.Equal("char-3", source.Characters.Add(
                new Character("Fresh", 10, 0, 1), null));
        }

        [Fact]
        public void Save_CreatesDirectoriesAndNoTempLeftover()
        {
            JsonWorldStore store = new(PathOf("nested/world.json"));
            store.Save(Seeded());
            Assert.True(File.Exists(PathOf("nested/world.json")));
            Assert.False(File.Exists(PathOf("nested/world.json.tmp")));
        }

        [Fact]
        public void Load_MissingFile_IsReported()
        {
            JsonWorldStore store = new(PathOf("absent.json"));
            Assert.Throws<DataException>(() => store.Load(new CharacterWorld()));
        }

        [Fact]
        public void Load_MalformedFile_IsReported()
        {
            File.WriteAllText(PathOf("broken.json"), "{ not json");
            JsonWorldStore store = new(PathOf("broken.json"));
            Assert.Throws<DataException>(() => store.Load(new CharacterWorld()));
        }

        [Fact]
        public void Load_InconsistentReferences_AreReported()
        {
            File.WriteAllText(PathOf("bad-ref.json"),
                """{"Characters":[{"Id":"char-1","Name":"Mage","Health":10,"MaxHealth":10,"BaseArmor":0,"BaseAttack":1,"GraphicsUrl":null,"ItemIds":["item-99"]}],"Items":[],"Abilities":[],"Books":{}}""");
            JsonWorldStore store = new(PathOf("bad-ref.json"));
            Assert.Throws<DataException>(() => store.Load(new CharacterWorld()));
        }

        [Fact]
        public void Load_DuplicateId_IsReported()
        {
            File.WriteAllText(PathOf("dup.json"),
                """{"Characters":[{"Id":"char-1","Name":"A","Health":10,"MaxHealth":10,"BaseArmor":0,"BaseAttack":1,"GraphicsUrl":null,"ItemIds":[]},{"Id":"char-1","Name":"B","Health":10,"MaxHealth":10,"BaseArmor":0,"BaseAttack":1,"GraphicsUrl":null,"ItemIds":[]}],"Items":[],"Abilities":[],"Books":{}}""");
            JsonWorldStore store = new(PathOf("dup.json"));
            Assert.Throws<DataException>(() => store.Load(new CharacterWorld()));
        }
    }
}
