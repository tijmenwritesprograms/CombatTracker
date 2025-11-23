using System.ComponentModel.DataAnnotations;
using CombatTracker.WebAssembly.Models;

namespace CombatTracker.WebAssembly.Tests.ModelValidation;

/// <summary>
/// Tests for Party model validation attributes
/// </summary>
public class PartyValidationTests
{
    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void Party_ValidData_ShouldPassValidation()
    {
        // Arrange
        var party = new Party
        {
            Id = 1,
            Name = "The Brave Adventurers",
            Characters = new List<Character>()
        };

        // Act
        var results = ValidateModel(party);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Party_EmptyName_ShouldFailValidation()
    {
        // Arrange
        var party = new Party
        {
            Name = ""
        };

        // Act
        var results = ValidateModel(party);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Party_NameTooLong_ShouldFailValidation()
    {
        // Arrange
        var party = new Party
        {
            Name = new string('A', 101) // 101 characters
        };

        // Act
        var results = ValidateModel(party);

        // Assert
        Assert.NotEmpty(results);
        Assert.Contains(results, r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Party_WithCharacters_ShouldPassValidation()
    {
        // Arrange
        var party = new Party
        {
            Id = 1,
            Name = "The Brave Adventurers",
            Characters = new List<Character>
            {
                new Character
                {
                    Id = 1,
                    Name = "Eldrid",
                    Class = "Fighter",
                    Level = 5,
                    HpMax = 50,
                    AC = 18
                }
            }
        };

        // Act
        var results = ValidateModel(party);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void Party_EmptyCharacterList_ShouldPassValidation()
    {
        // Arrange
        var party = new Party
        {
            Id = 1,
            Name = "Empty Party",
            Characters = new List<Character>()
        };

        // Act
        var results = ValidateModel(party);

        // Assert
        Assert.Empty(results);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("The Adventurers")]
    [InlineData("Campaign: Curse of Strahd")]
    public void Party_ValidNames_ShouldPassValidation(string name)
    {
        // Arrange
        var party = new Party
        {
            Name = name
        };

        // Act
        var results = ValidateModel(party);

        // Assert
        Assert.Empty(results);
    }
}
