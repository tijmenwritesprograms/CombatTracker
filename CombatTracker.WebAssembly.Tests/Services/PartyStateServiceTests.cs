using CombatTracker.WebAssembly.Models;
using CombatTracker.WebAssembly.Services;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Services;

/// <summary>
/// Tests for the PartyStateService
/// </summary>
public class PartyStateServiceTests
{
    [Fact]
    public void CreateParty_ShouldAddPartyToList()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());

        // Act
        var party = service.CreateParty("Test Party");

        // Assert
        Assert.NotNull(party);
        Assert.Equal("Test Party", party.Name);
        Assert.Equal(1, party.Id);
        Assert.Single(service.GetAllParties());
    }

    [Fact]
    public void CreateParty_ShouldAssignIncrementingIds()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());

        // Act
        var party1 = service.CreateParty("Party 1");
        var party2 = service.CreateParty("Party 2");

        // Assert
        Assert.Equal(1, party1.Id);
        Assert.Equal(2, party2.Id);
    }

    [Fact]
    public void GetAllParties_ShouldReturnEmptyList_WhenNoPartiesExist()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());

        // Act
        var parties = service.GetAllParties();

        // Assert
        Assert.Empty(parties);
    }

    [Fact]
    public void GetPartyById_ShouldReturnParty_WhenPartyExists()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = service.CreateParty("Test Party");

        // Act
        var result = service.GetPartyById(party.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Party", result.Name);
    }

    [Fact]
    public void GetPartyById_ShouldReturnNull_WhenPartyDoesNotExist()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());

        // Act
        var result = service.GetPartyById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void UpdateParty_ShouldUpdatePartyName()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = service.CreateParty("Original Name");

        // Act
        party.Name = "Updated Name";
        service.UpdateParty(party);

        // Assert
        var updatedParty = service.GetPartyById(party.Id);
        Assert.Equal("Updated Name", updatedParty!.Name);
    }

    [Fact]
    public void DeleteParty_ShouldRemovePartyFromList()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = service.CreateParty("Test Party");

        // Act
        service.DeleteParty(party.Id);

        // Assert
        Assert.Empty(service.GetAllParties());
    }

    [Fact]
    public void AddCharacter_ShouldAddCharacterToParty()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = service.CreateParty("Test Party");
        var character = new Character
        {
            Name = "Test Character",
            Class = "Fighter",
            Level = 5,
            HpMax = 40,
            HpCurrent = 40,
            AC = 18
        };

        // Act
        var result = service.AddCharacter(party.Id, character);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        var updatedParty = service.GetPartyById(party.Id);
        Assert.Single(updatedParty!.Characters);
        Assert.Equal("Test Character", updatedParty.Characters[0].Name);
    }

    [Fact]
    public void AddCharacter_ShouldThrowException_WhenPartyDoesNotExist()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var character = new Character { Name = "Test" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.AddCharacter(999, character));
    }

    [Fact]
    public void AddCharacter_ShouldAssignIncrementingIds()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = service.CreateParty("Test Party");
        var char1 = new Character { Name = "Character 1", Class = "Fighter", Level = 1, HpMax = 10, HpCurrent = 10, AC = 15 };
        var char2 = new Character { Name = "Character 2", Class = "Wizard", Level = 1, HpMax = 8, HpCurrent = 8, AC = 12 };

        // Act
        var result1 = service.AddCharacter(party.Id, char1);
        var result2 = service.AddCharacter(party.Id, char2);

        // Assert
        Assert.Equal(1, result1.Id);
        Assert.Equal(2, result2.Id);
    }

    [Fact]
    public void UpdateCharacter_ShouldUpdateCharacterProperties()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = service.CreateParty("Test Party");
        var character = new Character
        {
            Name = "Original Name",
            Class = "Fighter",
            Level = 5,
            HpMax = 40,
            HpCurrent = 40,
            AC = 18
        };
        service.AddCharacter(party.Id, character);

        // Act
        character.Name = "Updated Name";
        character.Level = 6;
        service.UpdateCharacter(party.Id, character);

        // Assert
        var updatedParty = service.GetPartyById(party.Id);
        var updatedCharacter = updatedParty!.Characters[0];
        Assert.Equal("Updated Name", updatedCharacter.Name);
        Assert.Equal(6, updatedCharacter.Level);
    }

    [Fact]
    public void UpdateCharacter_ShouldThrowException_WhenPartyDoesNotExist()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var character = new Character { Id = 1, Name = "Test" };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.UpdateCharacter(999, character));
    }

    [Fact]
    public void DeleteCharacter_ShouldRemoveCharacterFromParty()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = service.CreateParty("Test Party");
        var character = new Character
        {
            Name = "Test Character",
            Class = "Fighter",
            Level = 5,
            HpMax = 40,
            HpCurrent = 40,
            AC = 18
        };
        service.AddCharacter(party.Id, character);

        // Act
        service.DeleteCharacter(party.Id, character.Id);

        // Assert
        var updatedParty = service.GetPartyById(party.Id);
        Assert.Empty(updatedParty!.Characters);
    }

    [Fact]
    public void DeleteCharacter_ShouldThrowException_WhenPartyDoesNotExist()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => service.DeleteCharacter(999, 1));
    }

    [Fact]
    public void OnChange_ShouldBeInvoked_WhenPartyIsCreated()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var eventInvoked = false;
        service.OnChange += () => eventInvoked = true;

        // Act
        service.CreateParty("Test Party");

        // Assert
        Assert.True(eventInvoked);
    }

    [Fact]
    public void OnChange_ShouldBeInvoked_WhenPartyIsDeleted()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = service.CreateParty("Test Party");
        var eventInvoked = false;
        service.OnChange += () => eventInvoked = true;

        // Act
        service.DeleteParty(party.Id);

        // Assert
        Assert.True(eventInvoked);
    }

    [Fact]
    public void OnChange_ShouldBeInvoked_WhenCharacterIsAdded()
    {
        // Arrange
        var service = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = service.CreateParty("Test Party");
        var eventInvoked = false;
        service.OnChange += () => eventInvoked = true;

        // Act
        service.AddCharacter(party.Id, new Character { Name = "Test", Class = "Fighter", Level = 1, HpMax = 10, HpCurrent = 10, AC = 15 });

        // Assert
        Assert.True(eventInvoked);
    }
}
