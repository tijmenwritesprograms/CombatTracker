using System.ComponentModel.DataAnnotations;
using CombatTracker.WebAssembly.Models;

namespace CombatTracker.WebAssembly.Tests.ModelValidation;

/// <summary>
/// Tests for AbilityScores model and modifier calculations
/// </summary>
public class AbilityScoresTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void AbilityScores_DefaultValues_ShouldBe10()
    {
        // Arrange & Act
        var abilities = new AbilityScores();

        // Assert
        Assert.Equal(10, abilities.Strength);
        Assert.Equal(10, abilities.Dexterity);
        Assert.Equal(10, abilities.Constitution);
        Assert.Equal(10, abilities.Intelligence);
        Assert.Equal(10, abilities.Wisdom);
        Assert.Equal(10, abilities.Charisma);
    }

    [Fact]
    public void AbilityScores_DefaultModifiers_ShouldBeZero()
    {
        // Arrange & Act
        var abilities = new AbilityScores();

        // Assert
        Assert.Equal(0, abilities.StrengthModifier);
        Assert.Equal(0, abilities.DexterityModifier);
        Assert.Equal(0, abilities.ConstitutionModifier);
        Assert.Equal(0, abilities.IntelligenceModifier);
        Assert.Equal(0, abilities.WisdomModifier);
        Assert.Equal(0, abilities.CharismaModifier);
    }

    [Theory]
    [InlineData(1, -4)]   // (1-10)/2 = -9/2 = -4 (integer division)
    [InlineData(8, -1)]   // (8-10)/2 = -2/2 = -1
    [InlineData(9, 0)]    // (9-10)/2 = -1/2 = 0 (integer division rounds toward zero in C#)
    [InlineData(10, 0)]
    [InlineData(11, 0)]
    [InlineData(12, 1)]
    [InlineData(13, 1)]
    [InlineData(18, 4)]
    [InlineData(20, 5)]
    [InlineData(30, 10)]
    public void GetModifier_ShouldCalculateCorrectly(int score, int expectedModifier)
    {
        // Act
        var modifier = AbilityScores.GetModifier(score);

        // Assert
        Assert.Equal(expectedModifier, modifier);
    }

    [Fact]
    public void AbilityScores_StrengthModifier_ShouldCalculateCorrectly()
    {
        // Arrange
        var abilities = new AbilityScores { Strength = 18 };

        // Act & Assert
        Assert.Equal(4, abilities.StrengthModifier);
    }

    [Fact]
    public void AbilityScores_DexterityModifier_ShouldCalculateCorrectly()
    {
        // Arrange
        var abilities = new AbilityScores { Dexterity = 14 };

        // Act & Assert
        Assert.Equal(2, abilities.DexterityModifier);
    }

    [Fact]
    public void AbilityScores_ConstitutionModifier_ShouldCalculateCorrectly()
    {
        // Arrange
        var abilities = new AbilityScores { Constitution = 16 };

        // Act & Assert
        Assert.Equal(3, abilities.ConstitutionModifier);
    }

    [Fact]
    public void AbilityScores_IntelligenceModifier_ShouldCalculateCorrectly()
    {
        // Arrange
        var abilities = new AbilityScores { Intelligence = 8 };

        // Act & Assert
        Assert.Equal(-1, abilities.IntelligenceModifier);
    }

    [Fact]
    public void AbilityScores_WisdomModifier_ShouldCalculateCorrectly()
    {
        // Arrange
        var abilities = new AbilityScores { Wisdom = 12 };

        // Act & Assert
        Assert.Equal(1, abilities.WisdomModifier);
    }

    [Fact]
    public void AbilityScores_CharismaModifier_ShouldCalculateCorrectly()
    {
        // Arrange
        var abilities = new AbilityScores { Charisma = 20 };

        // Act & Assert
        Assert.Equal(5, abilities.CharismaModifier);
    }

    [Fact]
    public void AbilityScores_ValidRange_ShouldPassValidation()
    {
        // Arrange
        var abilities = new AbilityScores
        {
            Strength = 15,
            Dexterity = 14,
            Constitution = 13,
            Intelligence = 12,
            Wisdom = 10,
            Charisma = 8
        };

        // Act
        var results = ValidateModel(abilities);

        // Assert
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(31)]
    public void AbilityScores_InvalidStrength_ShouldFailValidation(int strength)
    {
        // Arrange
        var abilities = new AbilityScores { Strength = strength };

        // Act
        var results = ValidateModel(abilities);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Strength"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(31)]
    public void AbilityScores_InvalidDexterity_ShouldFailValidation(int dexterity)
    {
        // Arrange
        var abilities = new AbilityScores { Dexterity = dexterity };

        // Act
        var results = ValidateModel(abilities);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Dexterity"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(31)]
    public void AbilityScores_InvalidConstitution_ShouldFailValidation(int constitution)
    {
        // Arrange
        var abilities = new AbilityScores { Constitution = constitution };

        // Act
        var results = ValidateModel(abilities);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Constitution"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    public void AbilityScores_ValidBoundaries_ShouldPassValidation(int score)
    {
        // Arrange
        var abilities = new AbilityScores
        {
            Strength = score,
            Dexterity = score,
            Constitution = score,
            Intelligence = score,
            Wisdom = score,
            Charisma = score
        };

        // Act
        var results = ValidateModel(abilities);

        // Assert
        Assert.Empty(results);
    }
}
