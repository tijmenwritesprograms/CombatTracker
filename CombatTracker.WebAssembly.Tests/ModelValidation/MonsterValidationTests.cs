using System.ComponentModel.DataAnnotations;
using CombatTracker.WebAssembly.Models;

namespace CombatTracker.WebAssembly.Tests.ModelValidation;

/// <summary>
/// Tests for Monster model validation attributes
/// </summary>
public class MonsterValidationTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void Monster_ValidData_ShouldPassValidation()
    {
        // Arrange
        var monster = new Monster
        {
            Id = 1,
            Name = "Goblin",
            Size = "Small",
            Type = "Humanoid",
            Subtype = "Goblinoid",
            Alignment = "Neutral Evil",
            AC = 15,
            ArmorType = "Leather Armor, Shield",
            Hp = 7,
            HpFormula = "2d6",
            Speed = 30,
            InitiativeModifier = 2,
            ChallengeRating = "1/4",
            ExperiencePoints = 50,
            ProficiencyBonus = 2
        };

        // Act
        var results = ValidateModel(monster);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Monster_EmptyName_ShouldFailValidation()
    {
        // Arrange
        var monster = new Monster
        {
            Name = "",
            Type = "Humanoid",
            AC = 15,
            Hp = 7
        };

        // Act
        var results = ValidateModel(monster);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Monster_EmptyType_ShouldFailValidation()
    {
        // Arrange
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "",
            AC = 15,
            Hp = 7
        };

        // Act
        var results = ValidateModel(monster);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Type"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(31)]
    [InlineData(-1)]
    public void Monster_InvalidAC_ShouldFailValidation(int ac)
    {
        // Arrange
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            AC = ac,
            Hp = 7
        };

        // Act
        var results = ValidateModel(monster);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("AC"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Monster_InvalidHp_ShouldFailValidation(int hp)
    {
        // Arrange
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            AC = 15,
            Hp = hp
        };

        // Act
        var results = ValidateModel(monster);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Hp"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(201)]
    public void Monster_InvalidSpeed_ShouldFailValidation(int speed)
    {
        // Arrange
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            AC = 15,
            Hp = 7,
            Speed = speed
        };

        // Act
        var results = ValidateModel(monster);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Speed"));
    }

    [Theory]
    [InlineData(-6)]
    [InlineData(11)]
    public void Monster_InvalidInitiativeModifier_ShouldFailValidation(int initiativeModifier)
    {
        // Arrange
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            AC = 15,
            Hp = 7,
            InitiativeModifier = initiativeModifier
        };

        // Act
        var results = ValidateModel(monster);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("InitiativeModifier"));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(9)]
    public void Monster_ValidProficiencyBonus_ShouldPassValidation(int proficiencyBonus)
    {
        // Arrange
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            AC = 15,
            Hp = 7,
            ProficiencyBonus = proficiencyBonus
        };

        // Act
        var results = ValidateModel(monster);

        // Assert
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(0)]
    public void Monster_InvalidProficiencyBonus_ShouldFailValidation(int proficiencyBonus)
    {
        // Arrange
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            AC = 15,
            Hp = 7,
            ProficiencyBonus = proficiencyBonus
        };

        // Act
        var results = ValidateModel(monster);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("ProficiencyBonus"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(6)]
    public void Monster_InvalidLegendaryActionsPerRound_ShouldFailValidation(int legendaryActions)
    {
        // Arrange
        var monster = new Monster
        {
            Name = "Dragon",
            Type = "Dragon",
            AC = 20,
            Hp = 200,
            LegendaryActionsPerRound = legendaryActions
        };

        // Act
        var results = ValidateModel(monster);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("LegendaryActionsPerRound"));
    }

    [Fact]
    public void Monster_WithGroupIdAndInstanceNumber_ShouldPassValidation()
    {
        // Arrange
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            AC = 15,
            Hp = 7,
            GroupId = 1,
            InstanceNumber = 2
        };

        // Act
        var results = ValidateModel(monster);

        // Assert
        Assert.Empty(results);
    }
}
