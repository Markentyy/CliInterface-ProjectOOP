using CliInterface.Core.GameSystem;
using Xunit;

namespace Tests.Core
{
    /// <summary>
    /// Combat math and character state checks of the game system.
    /// </summary>
    public sealed class CharacterTests
    {
        /// <summary>
        /// Builds a simple character.
        /// </summary>
        /// <param name="attack">The base attack.</param>
        /// <param name="armor">The base armor.</param>
        /// <returns>The character.</returns>
        private static Character Hero(int attack = 10, int armor = 5) =>
            new("Hero", 100, armor, attack);

        [Fact]
        public void NewCharacter_HasFullHealthAndGraphicsLink()
        {
            Character c = new("Gate", 50, 3, 7, "https://example.com/g.png");
            Assert.Equal("Gate", c.Name);
            Assert.Equal(50, c.Health);
            Assert.Equal("https://example.com/g.png", c.GraphicsUrl);
            Assert.False(c.IsDefeated);
        }

        [Fact]
        public void NewCharacter_WithoutLink_HasNullGraphics()
        {
            Assert.Null(Hero().GraphicsUrl);
        }

        [Fact]
        public void BlankName_IsRejected()
        {
            Assert.Throws<ArgumentException>(() => new Character("  ", 10, 0, 0));
        }

        [Fact]
        public void InvalidStats_AreRejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Character("A", 0, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Character("A", 10, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Character("A", 10, 0, -1));
        }

        [Fact]
        public void Equip_AddsBonusesToTotals()
        {
            Character c = Hero();
            c.Equip(new Equipment("Sword", 3, 2));
            Assert.Equal(13, c.TotalAttack);
            Assert.Equal(7, c.TotalArmor);
        }

        [Fact]
        public void Defend_AddsArmorUntilNextAttack()
        {
            Character hero = Hero();
            hero.Defend();
            Assert.Equal(10, hero.TotalArmor);
            hero.Attack(new Character("Goblin", 30, 0, 0));
            Assert.False(hero.IsDefending);
        }

        [Fact]
        public void Attack_DamagesTargetThroughArmor()
        {
            Character hero = Hero(attack: 10);
            Character goblin = new("Goblin", 30, 2, 0);
            int damage = hero.Attack(goblin);
            Assert.Equal(8, damage);
            Assert.Equal(22, goblin.Health);
        }

        [Fact]
        public void Attack_OnDefeatedTarget_ClampsAtZero()
        {
            Character hero = Hero(attack: 100);
            Character goblin = new("Goblin", 5, 0, 0);
            hero.Attack(goblin);
            Assert.Equal(0, goblin.Health);
            Assert.True(goblin.IsDefeated);
        }

        [Fact]
        public void Heal_ClampsAtMax()
        {
            Character victim = Hero(armor: 5);
            Character striker = new("Striker", 100, 0, 50);
            striker.Attack(victim);
            Assert.Equal(55, victim.Health);
            victim.Heal(10);
            Assert.Equal(65, victim.Health);
            victim.Heal(100);
            Assert.Equal(100, victim.Health);
        }

        [Fact]
        public void Ability_DamageUsesMultiplier()
        {
            Character hero = Hero(attack: 10);
            Character goblin = new("Goblin", 100, 0, 0);
            var ability = new Ability("Slash", 3);
            int damage = hero.UseAbility(ability, goblin);
            Assert.Equal(30, damage);
        }

        [Fact]
        public void CombatResolver_NeverReturnsNegativeDamage() =>
            Assert.Equal(0, CombatResolver.CalculateDamage(1, 10));

        [Fact]
        public void CombatResolver_AbilityDamageIsPureMultiplier() =>
            Assert.Equal(24, CombatResolver.CalculateAbilityDamage(8, 3));
    }
}
