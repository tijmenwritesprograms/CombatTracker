using Bunit;
using CombatTracker.WebAssembly.Components.Shared;
using CombatTracker.WebAssembly.Models;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Components.Shared;

/// <summary>
/// Tests for the MonsterStatblockDisplay component.
/// </summary>
public class MonsterStatblockDisplayTests : TestContext
{
    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderEmptyMessage_WhenMonsterIsNull()
    {
        // Arrange & Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, null));

        // Assert
        var message = cut.Find("p.text-muted");
        Assert.Equal("No monster data available.", message.TextContent);
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderMonsterName_WhenMonsterProvided()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var nameElement = cut.Find("h3.monster-name");
        Assert.Contains("Test Orc", nameElement.TextContent);
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderBasicInfo_WhenMonsterProvided()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var subtitle = cut.Find("p.text-muted");
        Assert.Contains("Medium", subtitle.TextContent);
        Assert.Contains("Humanoid", subtitle.TextContent);
        Assert.Contains("Chaotic Evil", subtitle.TextContent);
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderArmorClass_WhenMonsterProvided()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("13", markup);
        Assert.Contains("hide armor", markup);
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderHitPoints_WhenMonsterProvided()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("15", markup);
        Assert.Contains("2d8 + 6", markup);
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderAbilityScores_WhenMonsterProvided()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("16", markup); // STR
        Assert.Contains("12", markup); // DEX
        Assert.Contains("16", markup); // CON (same as STR in test)
        Assert.Contains("7", markup);  // INT
        Assert.Contains("11", markup); // WIS
        Assert.Contains("10", markup); // CHA
        Assert.Contains("+3", markup); // STR modifier
        Assert.Contains("+1", markup); // DEX modifier
        Assert.Contains("-2", markup); // INT modifier
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderTraits_WhenMonsterHasTraits()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("Aggressive", markup);
        Assert.Contains("As a bonus action", markup);
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderActions_WhenMonsterHasActions()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("Greataxe", markup);
        Assert.Contains("Melee Weapon Attack", markup);
        Assert.Contains("+5 to hit", markup);
        Assert.Contains("reach 5 ft.", markup);
        Assert.Contains("1d12 + 3", markup);
        Assert.Contains("slashing", markup);
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderLegendaryActions_WhenMonsterHasLegendaryActions()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.LegendaryActions = new List<MonsterAction>
        {
            new MonsterAction
            {
                Name = "Detect",
                Description = "The monster makes a Wisdom (Perception) check.",
                LegendaryActionCost = 1
            },
            new MonsterAction
            {
                Name = "Attack",
                Description = "The monster makes one greataxe attack.",
                LegendaryActionCost = 2
            }
        };
        monster.LegendaryActionsPerRound = 3;

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("Legendary Actions", markup);
        Assert.Contains("3 legendary action", markup);
        Assert.Contains("Detect", markup);
        Assert.Contains("Attack", markup);
        Assert.Contains("Costs 2 Actions", markup);
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldNotRenderLegendaryActions_WhenMonsterHasNoLegendaryActions()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.LegendaryActions = new List<MonsterAction>();

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var markup = cut.Markup;
        Assert.DoesNotContain("Legendary Actions", markup);
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderChallengeRating_WhenMonsterProvided()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("1/2", markup);
        Assert.Contains("100 XP", markup);
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderSpeed_WhenMonsterProvided()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("30 ft.", markup);
    }

    [Fact]
    public void MonsterStatblockDisplay_ShouldRenderMultipleSpeeds_WhenMonsterHasMultipleSpeeds()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.FlySpeed = 60;
        monster.SwimSpeed = 40;

        // Act
        var cut = RenderComponent<MonsterStatblockDisplay>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var markup = cut.Markup;
        Assert.Contains("30 ft.", markup);
        Assert.Contains("fly 60 ft.", markup);
        Assert.Contains("swim 40 ft.", markup);
    }

    /// <summary>
    /// Helper method to create a test monster with basic properties.
    /// </summary>
    private static Monster CreateTestMonster()
    {
        return new Monster
        {
            Id = 1,
            Name = "Test Orc",
            Size = "Medium",
            Type = "Humanoid",
            Subtype = "Orc",
            Alignment = "Chaotic Evil",
            AC = 13,
            ArmorType = "hide armor",
            Hp = 15,
            HpFormula = "2d8 + 6",
            Speed = 30,
            Abilities = new AbilityScores
            {
                Strength = 16,
                Dexterity = 12,
                Constitution = 16,
                Intelligence = 7,
                Wisdom = 11,
                Charisma = 10
            },
            Skills = "Intimidation +2",
            Senses = "darkvision 60 ft.",
            Languages = "Common, Orc",
            ChallengeRating = "1/2",
            ExperiencePoints = 100,
            ProficiencyBonus = 2,
            Traits = new List<MonsterTrait>
            {
                new MonsterTrait
                {
                    Name = "Aggressive",
                    Description = "As a bonus action, the orc can move up to its speed toward a hostile creature that it can see."
                }
            },
            Actions = new List<MonsterAction>
            {
                new MonsterAction
                {
                    Name = "Greataxe",
                    Description = "Melee Weapon Attack: +5 to hit, reach 5 ft., one target. Hit: 9 (1d12 + 3) slashing damage.",
                    AttackType = "Melee Weapon Attack",
                    AttackBonus = 5,
                    Reach = 5,
                    Range = null,
                    DamageFormula = "1d12 + 3",
                    DamageType = "slashing"
                }
            }
        };
    }
}
