using Bunit;
using CombatTracker.WebAssembly.Components.CombatTrackerFolder;
using CombatTracker.WebAssembly.Models;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Components.CombatTracker;

/// <summary>
/// Tests for the MonsterQuickReference component
/// </summary>
public class MonsterQuickReferenceTests : TestContext
{
    [Fact]
    public void MonsterQuickReference_ShouldNotRender_WhenMonsterIsNull()
    {
        // Arrange & Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, null));

        // Assert
        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void MonsterQuickReference_ShouldRender_WhenMonsterProvided()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var row = cut.Find("tr.monster-quick-reference");
        Assert.NotNull(row);
    }

    [Fact]
    public void MonsterQuickReference_ShouldDisplayAC()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        Assert.Contains("AC:", cut.Markup);
        Assert.Contains("13", cut.Markup);
        Assert.Contains("hide armor", cut.Markup);
    }

    [Fact]
    public void MonsterQuickReference_ShouldDisplayHP()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        Assert.Contains("HP:", cut.Markup);
        Assert.Contains("15", cut.Markup);
        Assert.Contains("2d8 + 6", cut.Markup);
    }

    [Fact]
    public void MonsterQuickReference_ShouldDisplaySpeed()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.FlySpeed = 60;
        monster.SwimSpeed = 40;

        // Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        Assert.Contains("Speed:", cut.Markup);
        Assert.Contains("30 ft.", cut.Markup);
        Assert.Contains("fly 60 ft.", cut.Markup);
        Assert.Contains("swim 40 ft.", cut.Markup);
    }

    [Fact]
    public void MonsterQuickReference_ShouldDisplayChallengeRating()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        Assert.Contains("CR:", cut.Markup);
        Assert.Contains("1/2", cut.Markup);
    }

    [Fact]
    public void MonsterQuickReference_ShouldDisplayPrimaryActions()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        Assert.Contains("Primary Actions:", cut.Markup);
        Assert.Contains("Greataxe", cut.Markup);
        Assert.Contains("+5 to hit", cut.Markup);
        Assert.Contains("1d12 + 3", cut.Markup);
        Assert.Contains("slashing", cut.Markup);
    }

    [Fact]
    public void MonsterQuickReference_ShouldLimitActionsToThree()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Actions = new List<MonsterAction>
        {
            new MonsterAction { Name = "Action1", Description = "Test1" },
            new MonsterAction { Name = "Action2", Description = "Test2" },
            new MonsterAction { Name = "Action3", Description = "Test3" },
            new MonsterAction { Name = "Action4", Description = "Test4" },
            new MonsterAction { Name = "Action5", Description = "Test5" }
        };

        // Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        Assert.Contains("Action1", cut.Markup);
        Assert.Contains("Action2", cut.Markup);
        Assert.Contains("Action3", cut.Markup);
        Assert.DoesNotContain("Action4", cut.Markup);
        Assert.DoesNotContain("Action5", cut.Markup);
    }

    [Fact]
    public void MonsterQuickReference_ShouldNotDisplayActions_WhenNoneExist()
    {
        // Arrange
        var monster = CreateTestMonster();
        monster.Actions = new List<MonsterAction>();

        // Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        Assert.DoesNotContain("Primary Actions:", cut.Markup);
    }

    [Fact]
    public void MonsterQuickReference_ShouldHaveQuickReferenceTitle()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var title = cut.Find("h6.card-title");
        Assert.Contains("Quick Reference", title.TextContent);
    }

    [Fact]
    public void MonsterQuickReference_ShouldHaveCorrectStyling()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var card = cut.Find(".card.border-danger.bg-light");
        Assert.NotNull(card);
    }

    [Fact]
    public void MonsterQuickReference_ShouldSpanAllColumns()
    {
        // Arrange
        var monster = CreateTestMonster();

        // Act
        var cut = RenderComponent<MonsterQuickReference>(parameters => parameters
            .Add(p => p.Monster, monster));

        // Assert
        var td = cut.Find("td");
        Assert.Equal("8", td.GetAttribute("colspan"));
    }

    private static Monster CreateTestMonster()
    {
        return new Monster
        {
            Id = 1,
            Name = "Test Orc",
            AC = 13,
            ArmorType = "hide armor",
            Hp = 15,
            HpFormula = "2d8 + 6",
            Speed = 30,
            FlySpeed = 0,
            SwimSpeed = 0,
            ChallengeRating = "1/2",
            Actions = new List<MonsterAction>
            {
                new MonsterAction
                {
                    Name = "Greataxe",
                    Description = "Melee Weapon Attack: +5 to hit, reach 5 ft., one target. Hit: 9 (1d12 + 3) slashing damage.",
                    AttackType = "Melee Weapon Attack",
                    AttackBonus = 5,
                    Reach = 5,
                    DamageFormula = "1d12 + 3",
                    DamageType = "slashing"
                }
            }
        };
    }
}
