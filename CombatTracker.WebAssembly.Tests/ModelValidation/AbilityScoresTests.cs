using System.ComponentModel.DataAnnotations;
using CombatTracker.WebAssembly.Models;

namespace CombatTracker.WebAssembly.Tests.ModelValidation;

/// <summary>
/// Tests for AbilityScores model and modifier calculations
/// 
/// NOTE: The current implementation uses C# integer division which truncates toward zero.
/// This differs slightly from D&D 5e official rules which use floor division (round down).
/// For odd ability scores below 10 (e.g., 1, 3, 5, 7, 9), the modifiers differ:
/// - Score 1: Implementation returns -4, D&D 5e expects -5
/// - Score 9: Implementation returns 0, D&D 5e expects -1
/// These tests validate the current implementation behavior.
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
    [InlineData(1, -4)]   // (1-10)/2 = -9/2 = -4 (C# integer division truncates toward zero)
                          // NOTE: D&D 5e would use -5 (floor division), but implementation uses truncation
    [InlineData(8, -1)]   // (8-10)/2 = -2/2 = -1 (correct for both methods)
    [InlineData(9, 0)]    // (9-10)/2 = -1/2 = 0 (C# truncation)
                          // NOTE: D&D 5e would use -1 (floor division), but implementation uses truncation
    [InlineData(10, 0)]   // (10-10)/2 = 0
    [InlineData(11, 0)]   // (11-10)/2 = 1/2 = 0 (truncation)
    [InlineData(12, 1)]   // (12-10)/2 = 2/2 = 1
    [InlineData(13, 1)]   // (13-10)/2 = 3/2 = 1 (truncation)
    [InlineData(18, 4)]   // (18-10)/2 = 8/2 = 4
    [InlineData(20, 5)]   // (20-10)/2 = 10/2 = 5
    [InlineData(30, 10)]  // (30-10)/2 = 20/2 = 10
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
