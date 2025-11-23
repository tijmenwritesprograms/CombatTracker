using System.ComponentModel.DataAnnotations;
using CombatTracker.WebAssembly.Models;

namespace CombatTracker.WebAssembly.Tests.ModelValidation;

/// <summary>
/// Tests for Character model validation attributes
/// </summary>
public class CharacterValidationTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void Character_ValidData_ShouldPassValidation()
    {
        // Arrange
        var character = new Character
        {
            Id = 1,
            Name = "Eldrid Windspear",
            Class = "Fighter",
            Level = 5,
            HpCurrent = 45,
            HpMax = 50,
            AC = 18,
            InitiativeModifier = 2,
            Notes = "Tank character with high AC"
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Character_EmptyName_ShouldFailValidation()
    {
        // Arrange
        var character = new Character
        {
            Name = "",
            Class = "Fighter",
            Level = 5,
            HpMax = 50,
            AC = 18
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Character_NameTooLong_ShouldFailValidation()
    {
        // Arrange
        var character = new Character
        {
            Name = new string('A', 101), // 101 characters
            Class = "Fighter",
            Level = 5,
            HpMax = 50,
            AC = 18
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Character_EmptyClass_ShouldFailValidation()
    {
        // Arrange
        var character = new Character
        {
            Name = "Eldrid",
            Class = "",
            Level = 5,
            HpMax = 50,
            AC = 18
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Class"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    [InlineData(-1)]
    public void Character_InvalidLevel_ShouldFailValidation(int level)
    {
        // Arrange
        var character = new Character
        {
            Name = "Eldrid",
            Class = "Fighter",
            Level = level,
            HpMax = 50,
            AC = 18
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Level"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(20)]
    public void Character_ValidLevel_ShouldPassValidation(int level)
    {
        // Arrange
        var character = new Character
        {
            Name = "Eldrid",
            Class = "Fighter",
            Level = level,
            HpMax = 50,
            AC = 18
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(-1)]
    public void Character_NegativeCurrentHp_ShouldFailValidation(int hpCurrent)
    {
        // Arrange
        var character = new Character
        {
            Name = "Eldrid",
            Class = "Fighter",
            Level = 5,
            HpCurrent = hpCurrent,
            HpMax = 50,
            AC = 18
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("HpCurrent"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Character_InvalidMaxHp_ShouldFailValidation(int hpMax)
    {
        // Arrange
        var character = new Character
        {
            Name = "Eldrid",
            Class = "Fighter",
            Level = 5,
            HpCurrent = 50,
            HpMax = hpMax,
            AC = 18
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("HpMax"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(31)]
    [InlineData(-1)]
    public void Character_InvalidAC_ShouldFailValidation(int ac)
    {
        // Arrange
        var character = new Character
        {
            Name = "Eldrid",
            Class = "Fighter",
            Level = 5,
            HpMax = 50,
            AC = ac
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("AC"));
    }

    [Theory]
    [InlineData(-6)]
    [InlineData(11)]
    public void Character_InvalidInitiativeModifier_ShouldFailValidation(int initiativeModifier)
    {
        // Arrange
        var character = new Character
        {
            Name = "Eldrid",
            Class = "Fighter",
            Level = 5,
            HpMax = 50,
            AC = 18,
            InitiativeModifier = initiativeModifier
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("InitiativeModifier"));
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(0)]
    [InlineData(10)]
    public void Character_ValidInitiativeModifier_ShouldPassValidation(int initiativeModifier)
    {
        // Arrange
        var character = new Character
        {
            Name = "Eldrid",
            Class = "Fighter",
            Level = 5,
            HpMax = 50,
            AC = 18,
            InitiativeModifier = initiativeModifier
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Character_NotesTooLong_ShouldFailValidation()
    {
        // Arrange
        var character = new Character
        {
            Name = "Eldrid",
            Class = "Fighter",
            Level = 5,
            HpMax = 50,
            AC = 18,
            Notes = new string('A', 1001) // 1001 characters
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Notes"));
    }

    [Fact]
    public void Character_NullNotes_ShouldPassValidation()
    {
        // Arrange
        var character = new Character
        {
            Name = "Eldrid",
            Class = "Fighter",
            Level = 5,
            HpMax = 50,
            AC = 18,
            Notes = null
        };

        // Act
        var results = ValidateModel(character);

        // Assert
        Assert.Empty(results);
    }
}
