using CliApp = CliInterface.Cli.Cli;
using CliInterface.Cli.Engine;
using CliInterface.Cli.Rendering;
using CliInterface.Cli.Strategies;
using CliInterface.Core.Catalog;
using CliInterface.Core.GameSystem;
using CliInterface.Data;
using Xunit;

namespace Tests.Cli
{
    /// <summary>
    /// Facade behavior: strategy registration, dispatch, error mapping,
    /// display and exit handling over a scripted fake console.
    /// </summary>
    public sealed class CliFacadeTests
    {
        /// <summary>
        /// Initializes a fresh world, store and facade.
        /// </summary>
        public CliFacadeTests()
        {
            World = new CharacterWorld();
            Console = new FakeConsole(Array.Empty<string>());
            Facade = new CliApp(Console);
            Facade.UseStrategy(new InfoStrategy(
                World, new CharacterTableRenderer(), new CharacterCardRenderer()));
            Facade.UseStrategy(new TransformStrategy(World, null));
            Facade.UseStrategy(new InfraStrategy(World, new MemoryStore()));
        }

        private CharacterWorld World { get; }
        private FakeConsole Console { get; }
        private CliApp Facade { get; }

        /// <summary>
        /// Throws on any save or load: proves tests without persistence.
        /// </summary>
        internal sealed class MemoryStore : IWorldStore
        {
            /// <summary>
            /// Gets the location label.
            /// </summary>
            public string Location => "<memory>";

            /// <summary>
            /// No save behavior in this test double.
            /// </summary>
            /// <param name="world">The world.</param>
            public void Save(CharacterWorld world) => throw new DataException("save blocked");

            /// <summary>
            /// No load behavior in this test double.
            /// </summary>
            /// <param name="world">The world.</param>
            public void Load(CharacterWorld world) => throw new DataException("load blocked");
        }

        [Fact]
        public void ExecCommand_UnknownVerb_PrintsHint()
        {
            Facade.ExecCommand("bogus");
            Assert.True(Console.Contains("Unknown command 'bogus'. Type 'help'."));
        }

        [Fact]
        public void ExecCommand_EmptyLine_IsIgnored()
        {
            Facade.ExecCommand("   ");
            Assert.Equal(string.Empty, Console.Output);
        }

        [Fact]
        public void ExecCommand_BadSyntax_PrintsError()
        {
            Facade.ExecCommand("ls --wat x");
            Assert.True(Console.Contains("Error: Unknown option --wat"));
        }

        [Fact]
        public void ExecCommand_Exit_SetsQuitFlag()
        {
            Facade.ExecCommand("exit");
            Assert.True(Facade.QuitRequested);
        }

        [Fact]
        public void Ls_EmptyCatalog_PromptsCreation()
        {
            Facade.ExecCommand("ls");
            Assert.True(Console.Contains("No characters. Create one with: create char"));
        }

        [Fact]
        public void Ls_Characters_RendersAlignedTable()
        {
            World.Characters.Add(new Character("Mage", 50, 2, 6, "https://g/m.png"), "char-1");
            World.Characters.Add(new Character("Goblin", 10, 0, 3), "char-2");
            Character striker = new("Striker", 10, 0, 7);
            World.Characters.Add(striker, "char-3");
            striker.Attack(World.Characters.GetById("char-2"));
            Facade.ExecCommand("ls");
            Assert.True(Console.Contains("[char-1]  Mage"));
            Assert.True(Console.Contains("########## 50/50"));
            Assert.True(Console.Contains("###------- 3/10"));
        }

        [Fact]
        public void Info_ByName_RendersCardWithGraphics()
        {
            World.Characters.Add(new Character("Mage", 50, 2, 6, "https://g/m.png"), "char-1");
            Facade.ExecCommand("info Mage");
            Assert.True(Console.Contains("[char-1] Mage"));
            Assert.True(Console.Contains("Graphics: https://g/m.png"));
            Assert.True(Console.Contains("Items: -"));
        }

        [Fact]
        public void Info_UnknownObject_IsReported()
        {
            Facade.ExecCommand("info nobody");
            Assert.True(Console.Contains("Error: No catalog object 'nobody'. Try: ls"));
        }

        [Fact]
        public void Info_AmbiguousName_IsReported()
        {
            World.Characters.Add(new Character("Twin", 10, 0, 1), "char-1");
            World.Characters.Add(new Character("Twin", 10, 0, 1), "char-2");
            Facade.ExecCommand("info Twin");
            Assert.True(Console.Contains("Error: Ambiguous character name 'Twin': char-1, char-2"));
        }

        [Fact]
        public void CreateChar_DialogFlow_CreatesAndLists()
        {
            FakeConsole dialog = new(new[]
            {
                "Gatekeeper", "100", "5", "10", "https://g/gate.png", "",
            });
            CliApp facade = new(dialog);
            facade.UseStrategy(new TransformStrategy(World, null));
            facade.ExecCommand("create char");
            Assert.True(dialog.Contains("Created character 'Gatekeeper' [char-1]"));
            Assert.True(dialog.Contains("Graphics URL (empty to skip)"));

            facade.ExecCommand("ls");
            Assert.True(dialog.Contains("Gatekeeper"));
        }

        [Fact]
        public void Create_CancelKeyword_AbortsDialog()
        {
            FakeConsole dialog = new(new[] { "cancel" });
            CliApp facade = new(dialog);
            facade.UseStrategy(new TransformStrategy(World, null));
            facade.ExecCommand("create char");
            Assert.True(dialog.Contains("Cancelled."));
            Assert.Equal(0, World.Characters.Count);
        }

        [Fact]
        public void Add_EquipsItemAndUpdatesTable()
        {
            World.Items.Add(new Equipment("Sword", 3, 2), "item-1");
            World.Characters.Add(new Character("Mage", 50, 2, 6), "char-1");
            Facade.ExecCommand("add --char_id Mage --id Sword");
            Assert.True(Console.Contains("Mage equipped Sword (+3 ATK, +2 ARM)."));
            Facade.ExecCommand("ls");
            Assert.True(Console.Contains("9"));
        }

        [Fact]
        public void Add_Twice_DoesNotDuplicateItem()
        {
            World.Items.Add(new Equipment("Sword", 3, 2), "item-1");
            World.Characters.Add(new Character("Mage", 50, 2, 6), "char-1");
            Facade.ExecCommand("add --char_id Mage --id Sword");
            Facade.ExecCommand("add --char_id Mage --id Sword");
            Assert.True(Console.Contains("Mage already has Sword equipped."));
        }

        [Fact]
        public void Help_ListsCommandsGrouped()
        {
            Facade.ExecCommand("help");
            Assert.True(Console.Contains("Commands (grouped by strategy):"));
            Assert.True(Console.Contains("ls - List characters"));
            Assert.True(Console.Contains("info - Show one character"));
            Assert.True(Console.Contains("create - Create a character"));
            Assert.True(Console.Contains("add - Give an item"));
            Assert.True(Console.Contains("save - Save the catalog"));
            Assert.True(Console.Contains("load - Load the catalog"));
            Assert.True(Console.Contains("exit - Leave the interpreter."));
        }

        [Fact]
        public void UseStrategy_DuplicateCommand_IsRejected()
        {
            CliApp facade = new(new FakeConsole(Array.Empty<string>()));
            facade.UseStrategy(new TransformStrategy(World, null));
            Assert.Throws<CommandException>(() =>
                facade.UseStrategy(new TransformStrategy(World, null)));
        }

        [Fact]
        public void Repl_RunsUntilExit()
        {
            FakeConsole session = new(new[] { "ls", "exit" });
            CliApp facade = new(session);
            facade.UseStrategy(new InfoStrategy(
                World, new CharacterTableRenderer(), new CharacterCardRenderer()));
            CliInterface.Cli.Repl.Run(session, facade, "Character catalog.");
            Assert.True(session.Contains("Character catalog."));
            Assert.True(session.Contains("No characters."));
        }
    }
}
