using CombatTracker.Web.Models;
using CombatTracker.Web.Services;
using Xunit;

namespace CombatTracker.Web.Tests.Services;

/// <summary>
/// Tests for the CombatStateService
/// </summary>
public class CombatStateServiceTests
{
    [Fact]
    public void SelectParty_ShouldSetSelectedParty()
    {
        // Arrange
        var service = new CombatStateService();
        var party = new Party { Id = 1, Name = "Test Party" };

        // Act
        service.SelectParty(party);

        // Assert
        Assert.NotNull(service.SelectedParty);
        Assert.Equal("Test Party", service.SelectedParty.Name);
    }

    [Fact]
    public void SelectParty_ShouldAddCharactersToCombatants()
    {
        // Arrange
        var service = new CombatStateService();
        var party = new Party
        {
            Id = 1,
            Name = "Test Party",
            Characters = new List<Character>
            {
                new Character
                {
                    Id = 1,
                    Name = "Fighter",
                    Class = "Fighter",
                    Level = 5,
                    HpMax = 40,
                    HpCurrent = 40,
                    AC = 18,
                    InitiativeModifier = 2
                },
                new Character
                {
                    Id = 2,
                    Name = "Wizard",
                    Class = "Wizard",
                    Level = 5,
                    HpMax = 25,
                    HpCurrent = 25,
                    AC = 13,
                    InitiativeModifier = 3
                }
            }
        };

        // Act
        service.SelectParty(party);

        // Assert
        Assert.Equal(2, service.Combatants.Count);
        Assert.Contains("character-1", service.Combatants.Keys);
        Assert.Contains("character-2", service.Combatants.Keys);
        Assert.Equal("Fighter", service.Combatants["character-1"].Name);
        Assert.Equal("Wizard", service.Combatants["character-2"].Name);
    }

    [Fact]
    public void AddMonster_ShouldAddMonsterToList()
    {
        // Arrange
        var service = new CombatStateService();
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            Hp = 7,
            AC = 15,
            InitiativeModifier = 2
        };

        // Act
        var result = service.AddMonster(monster);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Single(service.GetMonsters());
        Assert.Equal("Goblin", service.GetMonsters()[0].Name);
    }

    [Fact]
    public void AddMonster_ShouldAssignIncrementingIds()
    {
        // Arrange
        var service = new CombatStateService();
        var monster1 = new Monster { Name = "Goblin 1", Type = "Humanoid", Hp = 7, AC = 15 };
        var monster2 = new Monster { Name = "Goblin 2", Type = "Humanoid", Hp = 7, AC = 15 };

        // Act
        var result1 = service.AddMonster(monster1);
        var result2 = service.AddMonster(monster2);

        // Assert
        Assert.Equal(1, result1.Id);
        Assert.Equal(2, result2.Id);
    }

    [Fact]
    public void AddMonster_ShouldAddMonsterToCombatants()
    {
        // Arrange
        var service = new CombatStateService();
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            Hp = 7,
            AC = 15,
            InitiativeModifier = 2
        };

        // Act
        service.AddMonster(monster);

        // Assert
        Assert.Single(service.Combatants);
        Assert.Contains("monster-1", service.Combatants.Keys);
        Assert.Equal("Goblin", service.Combatants["monster-1"].Name);
        Assert.Equal(7, service.Combatants["monster-1"].HpMax);
        Assert.False(service.Combatants["monster-1"].IsCharacter);
    }

    [Fact]
    public void RemoveMonster_ShouldRemoveMonsterFromList()
    {
        // Arrange
        var service = new CombatStateService();
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            Hp = 7,
            AC = 15
        };
        var added = service.AddMonster(monster);

        // Act
        service.RemoveMonster(added.Id);

        // Assert
        Assert.Empty(service.GetMonsters());
        Assert.Empty(service.Combatants);
    }

    [Fact]
    public void RollInitiativeForAll_ShouldSetInitiativeForAllCombatants()
    {
        // Arrange
        var service = new CombatStateService();
        var party = new Party
        {
            Id = 1,
            Name = "Test Party",
            Characters = new List<Character>
            {
                new Character
                {
                    Id = 1,
                    Name = "Fighter",
                    Class = "Fighter",
                    Level = 5,
                    HpMax = 40,
                    HpCurrent = 40,
                    AC = 18,
                    InitiativeModifier = 2
                }
            }
        };
        service.SelectParty(party);
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            Hp = 7,
            AC = 15,
            InitiativeModifier = 2
        };
        service.AddMonster(monster);

        // Act
        service.RollInitiativeForAll();

        // Assert
        Assert.NotEqual(0, service.Combatants["character-1"].Initiative);
        Assert.NotEqual(0, service.Combatants["monster-1"].Initiative);
        // Initiative should be between 1 (min roll) + modifier and 20 (max roll) + modifier
        Assert.InRange(service.Combatants["character-1"].Initiative, 3, 22);
        Assert.InRange(service.Combatants["monster-1"].Initiative, 3, 22);
    }

    [Fact]
    public void RollInitiativeForMonsters_ShouldSetInitiativeOnlyForMonsters()
    {
        // Arrange
        var service = new CombatStateService();
        var party = new Party
        {
            Id = 1,
            Name = "Test Party",
            Characters = new List<Character>
            {
                new Character
                {
                    Id = 1,
                    Name = "Fighter",
                    Class = "Fighter",
                    Level = 5,
                    HpMax = 40,
                    HpCurrent = 40,
                    AC = 18,
                    InitiativeModifier = 2
                }
            }
        };
        service.SelectParty(party);
        var monster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            Hp = 7,
            AC = 15,
            InitiativeModifier = 2
        };
        service.AddMonster(monster);

        // Act
        service.RollInitiativeForMonsters();

        // Assert
        // Characters should remain unchanged (0) while monsters receive initiative
        Assert.Equal(0, service.Combatants["character-1"].Initiative);
        Assert.NotEqual(0, service.Combatants["monster-1"].Initiative);
        Assert.InRange(service.Combatants["monster-1"].Initiative, 3, 22);
    }

    [Fact]
    public void SetInitiative_ShouldUpdateInitiativeForCombatant()
    {
        // Arrange
        var service = new CombatStateService();
        var party = new Party
        {
            Id = 1,
            Name = "Test Party",
            Characters = new List<Character>
            {
                new Character
                {
                    Id = 1,
                    Name = "Fighter",
                    Class = "Fighter",
                    Level = 5,
                    HpMax = 40,
                    HpCurrent = 40,
                    AC = 18,
                    InitiativeModifier = 2
                }
            }
        };
        service.SelectParty(party);

        // Act
        service.SetInitiative("character-1", 15);

        // Assert
        Assert.Equal(15, service.Combatants["character-1"].Initiative);
    }

    [Fact]
    public void IsValidForCombat_ShouldReturnFalse_WhenNoCombatants()
    {
        // Arrange
        var service = new CombatStateService();

        // Act
        var result = service.IsValidForCombat();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidForCombat_ShouldReturnTrue_WhenCombatantsExist()
    {
        // Arrange
        var service = new CombatStateService();
        var party = new Party
        {
            Id = 1,
            Name = "Test Party",
            Characters = new List<Character>
            {
                new Character
                {
                    Id = 1,
                    Name = "Fighter",
                    Class = "Fighter",
                    Level = 5,
                    HpMax = 40,
                    HpCurrent = 40,
                    AC = 18
                }
            }
        };
        service.SelectParty(party);

        // Act
        var result = service.IsValidForCombat();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Reset_ShouldClearAllCombatData()
    {
        // Arrange
        var service = new CombatStateService();
        var party = new Party
        {
            Id = 1,
            Name = "Test Party",
            Characters = new List<Character>
            {
                new Character
                {
                    Id = 1,
                    Name = "Fighter",
                    Class = "Fighter",
                    Level = 5,
                    HpMax = 40,
                    HpCurrent = 40,
                    AC = 18
                }
            }
        };
        service.SelectParty(party);
        service.AddMonster(new Monster { Name = "Goblin", Type = "Humanoid", Hp = 7, AC = 15 });

        // Act
        service.Reset();

        // Assert
        Assert.Null(service.SelectedParty);
        Assert.Empty(service.GetMonsters());
        Assert.Empty(service.Combatants);
    }

    [Fact]
    public void OnChange_ShouldBeInvoked_WhenPartyIsSelected()
    {
        // Arrange
        var service = new CombatStateService();
        var eventInvoked = false;
        service.OnChange += () => eventInvoked = true;
        var party = new Party { Id = 1, Name = "Test Party" };

        // Act
        service.SelectParty(party);

        // Assert
        Assert.True(eventInvoked);
    }

    [Fact]
    public void OnChange_ShouldBeInvoked_WhenMonsterIsAdded()
    {
        // Arrange
        var service = new CombatStateService();
        var eventInvoked = false;
        service.OnChange += () => eventInvoked = true;
        var monster = new Monster { Name = "Goblin", Type = "Humanoid", Hp = 7, AC = 15 };

        // Act
        service.AddMonster(monster);

        // Assert
        Assert.True(eventInvoked);
    }

    [Fact]
    public void OnChange_ShouldBeInvoked_WhenMonsterIsRemoved()
    {
        // Arrange
        var service = new CombatStateService();
        var monster = new Monster { Name = "Goblin", Type = "Humanoid", Hp = 7, AC = 15 };
        var added = service.AddMonster(monster);
        var eventInvoked = false;
        service.OnChange += () => eventInvoked = true;

        // Act
        service.RemoveMonster(added.Id);

        // Assert
        Assert.True(eventInvoked);
    }

    [Fact]
    public void SelectParty_ShouldRebuildCombatants_WhenPartyChanges()
    {
        // Arrange
        var service = new CombatStateService();
        var party1 = new Party
        {
            Id = 1,
            Name = "Party 1",
            Characters = new List<Character>
            {
                new Character { Id = 1, Name = "Fighter", Class = "Fighter", Level = 5, HpMax = 40, HpCurrent = 40, AC = 18 }
            }
        };
        var party2 = new Party
        {
            Id = 2,
            Name = "Party 2",
            Characters = new List<Character>
            {
                new Character { Id = 2, Name = "Wizard", Class = "Wizard", Level = 5, HpMax = 25, HpCurrent = 25, AC = 13 },
                new Character { Id = 3, Name = "Rogue", Class = "Rogue", Level = 5, HpMax = 30, HpCurrent = 30, AC = 15 }
            }
        };
        service.SelectParty(party1);

        // Act
        service.SelectParty(party2);

        // Assert
        Assert.Equal(2, service.Combatants.Count);
        Assert.Contains("character-2", service.Combatants.Keys);
        Assert.Contains("character-3", service.Combatants.Keys);
        Assert.DoesNotContain("character-1", service.Combatants.Keys);
    }

    [Fact]
    public void AddMonster_ShouldKeepExistingCharacters_WhenMonsterIsAdded()
    {
        // Arrange
        var service = new CombatStateService();
        var party = new Party
        {
            Id = 1,
            Name = "Test Party",
            Characters = new List<Character>
            {
                new Character { Id = 1, Name = "Fighter", Class = "Fighter", Level = 5, HpMax = 40, HpCurrent = 40, AC = 18 }
            }
        };
        service.SelectParty(party);

        // Act
        service.AddMonster(new Monster { Name = "Goblin", Type = "Humanoid", Hp = 7, AC = 15 });

        // Assert
        Assert.Equal(2, service.Combatants.Count);
        Assert.Contains("character-1", service.Combatants.Keys);
        Assert.Contains("monster-1", service.Combatants.Keys);
    }
}
