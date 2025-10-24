using CombatTracker.Web.Models;
using CombatTracker.Web.Services;
using Xunit;

namespace CombatTracker.Web.Tests.Services;

/// <summary>
/// Tests for combat tracking functionality in CombatStateService
/// </summary>
public class CombatTrackingTests
{
    [Fact]
    public void StartCombat_ShouldCreateActiveCombat()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.AddMonster(CreateTestMonster("Goblin"));
        service.RollInitiativeForAll();

        // Act
        service.StartCombat();

        // Assert
        Assert.NotNull(service.ActiveCombat);
        Assert.True(service.IsCombatActive);
        Assert.Equal(1, service.ActiveCombat.Round);
        Assert.Equal(0, service.ActiveCombat.TurnIndex);
    }

    [Fact]
    public void StartCombat_ShouldSortCombatantsByInitiativeDescending()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.AddMonster(CreateTestMonster("Goblin"));
        
        // Manually set initiatives for predictable order
        service.SetInitiative("character-1", 15);
        service.SetInitiative("monster-1", 10);

        // Act
        service.StartCombat();

        // Assert
        Assert.NotNull(service.ActiveCombat);
        Assert.Equal(2, service.ActiveCombat.Combatants.Count);
        Assert.Equal(15, service.ActiveCombat.Combatants[0].Initiative);
        Assert.Equal(10, service.ActiveCombat.Combatants[1].Initiative);
    }

    [Fact]
    public void StartCombat_ShouldInitializeCombatLog()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();

        // Act
        service.StartCombat();

        // Assert
        Assert.NotEmpty(service.CombatLog);
        Assert.Contains(service.CombatLog, log => log.Type == "Turn");
    }

    [Fact]
    public void GetCurrentCombatantInstance_ShouldReturnFirstCombatant()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();

        // Act
        var current = service.GetCurrentCombatantInstance();

        // Assert
        Assert.NotNull(current);
        Assert.Equal(Status.Alive, current.Status);
    }

    [Fact]
    public void GetCurrentCombatantData_ShouldReturnMatchingSetupData()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();

        // Act
        var currentData = service.GetCurrentCombatantData();

        // Assert
        Assert.NotNull(currentData);
        Assert.Equal("Fighter", currentData.Name);
    }

    [Fact]
    public void NextTurn_ShouldAdvanceTurnIndex()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.AddMonster(CreateTestMonster("Goblin"));
        service.RollInitiativeForAll();
        service.StartCombat();

        // Act
        service.NextTurn();

        // Assert
        Assert.Equal(1, service.ActiveCombat!.TurnIndex);
    }

    [Fact]
    public void NextTurn_ShouldIncrementRoundAfterLastCombatant()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();

        // Act
        service.NextTurn(); // Should wrap to round 2

        // Assert
        Assert.Equal(2, service.ActiveCombat!.Round);
        Assert.Equal(0, service.ActiveCombat.TurnIndex);
    }

    [Fact]
    public void NextTurn_ShouldSkipUnconsciousCombatants()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.AddMonster(CreateTestMonster("Goblin"));
        service.SetInitiative("character-1", 15);
        service.SetInitiative("monster-1", 10);
        service.StartCombat();

        // Make the second combatant unconscious
        service.ActiveCombat!.Combatants[1].Status = Status.Unconscious;

        // Act
        service.NextTurn(); // Should skip unconscious and wrap to round 2

        // Assert
        Assert.Equal(2, service.ActiveCombat.Round);
        Assert.Equal(0, service.ActiveCombat.TurnIndex);
    }

    [Fact]
    public void PreviousTurn_ShouldDecrementTurnIndex()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.AddMonster(CreateTestMonster("Goblin"));
        service.RollInitiativeForAll();
        service.StartCombat();
        service.NextTurn(); // Move to index 1

        // Act
        service.PreviousTurn();

        // Assert
        Assert.Equal(0, service.ActiveCombat!.TurnIndex);
    }

    [Fact]
    public void PreviousTurn_ShouldDecrementRoundWhenAtStart()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();
        service.NextTurn(); // Move to round 2

        // Act
        service.PreviousTurn(); // Should go back to round 1

        // Assert
        Assert.Equal(1, service.ActiveCombat!.Round);
    }

    [Fact]
    public void ApplyDamage_ShouldReduceHP()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();
        var initialHp = service.ActiveCombat!.Combatants[0].HpCurrent;

        // Act
        service.ApplyDamage(0, 10);

        // Assert
        Assert.Equal(initialHp - 10, service.ActiveCombat.Combatants[0].HpCurrent);
    }

    [Fact]
    public void ApplyDamage_ShouldNotReduceHPBelowZero()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();

        // Act
        service.ApplyDamage(0, 1000);

        // Assert
        Assert.Equal(0, service.ActiveCombat!.Combatants[0].HpCurrent);
    }

    [Fact]
    public void ApplyDamage_ShouldMarkAsUnconsciousWhenHPReachesZero()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();

        // Act
        service.ApplyDamage(0, 1000);

        // Assert
        Assert.Equal(Status.Unconscious, service.ActiveCombat!.Combatants[0].Status);
    }

    [Fact]
    public void ApplyDamage_ShouldAddDamageLogEntry()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();
        var initialLogCount = service.CombatLog.Count;

        // Act
        service.ApplyDamage(0, 10);

        // Assert
        Assert.True(service.CombatLog.Count > initialLogCount);
        Assert.Contains(service.CombatLog, log => log.Type == "Damage");
    }

    [Fact]
    public void ApplyDamage_ShouldAddStatusLogEntryWhenBecomeUnconscious()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();

        // Act
        service.ApplyDamage(0, 1000);

        // Assert
        Assert.Contains(service.CombatLog, log => log.Type == "Status" && log.Message.Contains("Unconscious"));
    }

    [Fact]
    public void ApplyHealing_ShouldIncreaseHP()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();
        service.ApplyDamage(0, 10);
        var damagedHp = service.ActiveCombat!.Combatants[0].HpCurrent;

        // Act
        service.ApplyHealing(0, 5);

        // Assert
        Assert.Equal(damagedHp + 5, service.ActiveCombat.Combatants[0].HpCurrent);
    }

    [Fact]
    public void ApplyHealing_ShouldNotExceedMaxHP()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();
        var maxHp = service.Combatants.Values.First().HpMax;

        // Act
        service.ApplyHealing(0, 1000);

        // Assert
        Assert.Equal(maxHp, service.ActiveCombat!.Combatants[0].HpCurrent);
    }

    [Fact]
    public void ApplyHealing_ShouldReviveUnconsciousCombatant()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();
        service.ApplyDamage(0, 1000); // Make unconscious

        // Act
        service.ApplyHealing(0, 5);

        // Assert
        Assert.Equal(Status.Alive, service.ActiveCombat!.Combatants[0].Status);
        Assert.Equal(5, service.ActiveCombat.Combatants[0].HpCurrent);
    }

    [Fact]
    public void ApplyHealing_ShouldAddHealLogEntry()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();
        service.ApplyDamage(0, 10);
        var initialLogCount = service.CombatLog.Count;

        // Act
        service.ApplyHealing(0, 5);

        // Assert
        Assert.True(service.CombatLog.Count > initialLogCount);
        Assert.Contains(service.CombatLog, log => log.Type == "Heal");
    }

    [Fact]
    public void ApplyHealing_ShouldAddStatusLogEntryWhenRevived()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();
        service.ApplyDamage(0, 1000); // Make unconscious

        // Act
        service.ApplyHealing(0, 5);

        // Assert
        Assert.Contains(service.CombatLog, log => log.Type == "Status" && log.Message.Contains("Alive"));
    }

    [Fact]
    public void GetCombatantsWithData_ShouldReturnAllCombatantsWithSetupData()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.AddMonster(CreateTestMonster("Goblin"));
        service.RollInitiativeForAll();
        service.StartCombat();

        // Act
        var combatants = service.GetCombatantsWithData();

        // Assert
        Assert.Equal(2, combatants.Count);
        Assert.NotNull(combatants[0].Instance);
        Assert.NotNull(combatants[0].Data);
        Assert.NotNull(combatants[1].Instance);
        Assert.NotNull(combatants[1].Data);
    }

    [Fact]
    public void EndCombat_ShouldClearActiveCombat()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();

        // Act
        service.EndCombat();

        // Assert
        Assert.Null(service.ActiveCombat);
        Assert.False(service.IsCombatActive);
        Assert.Empty(service.CombatLog);
    }

    [Fact]
    public void CombatLog_ShouldTrackRoundAndTurnIndex()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();

        // Act
        service.NextTurn();

        // Assert
        var lastEntry = service.CombatLog.Last();
        Assert.Equal(2, lastEntry.Round);
    }

    [Fact]
    public void CombatLog_ShouldIncludeTimestamp()
    {
        // Arrange
        var service = new CombatStateService();
        var party = CreateTestParty();
        service.SelectParty(party);
        service.RollInitiativeForAll();
        service.StartCombat();

        // Act
        var logEntry = service.CombatLog.First();

        // Assert
        Assert.NotEqual(default(DateTime), logEntry.Timestamp);
    }

    // Helper methods
    private static Party CreateTestParty()
    {
        return new Party
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
    }

    private static Monster CreateTestMonster(string name)
    {
        return new Monster
        {
            Name = name,
            Type = "Humanoid",
            Hp = 7,
            AC = 15,
            InitiativeModifier = 2
        };
    }
}
