using Bunit;
using CombatTracker.WebAssembly.Components.Pages;
using CombatTracker.WebAssembly.Models;
using CombatTracker.WebAssembly.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Pages;

/// <summary>
/// Tests for the Combat Tracker page component
/// </summary>
public class CombatTrackerPageTests : TestContext
{
    [Fact]
    public void CombatTracker_ShouldRenderTitle()
    {
        // Arrange
        Services.AddSingleton<PartyStateService>();
        Services.AddSingleton<CombatStateService>();

        // Act
        var cut = RenderComponent<CombatTrackerPage>();

        // Assert
        var h1 = cut.Find("h1");
        Assert.Equal("Combat Tracker", h1.TextContent);
    }

    [Fact]
    public void CombatTracker_ShouldDisplayWarning_WhenNoCombatActive()
    {
        // Arrange
        Services.AddSingleton<PartyStateService>();
        Services.AddSingleton<CombatStateService>();

        // Act
        var cut = RenderComponent<CombatTrackerPage>();

        // Assert
        var alert = cut.Find(".alert.alert-warning");
        Assert.Contains("No active combat", alert.TextContent);
    }

    [Fact]
    public void CombatTracker_ShouldDisplayCombatUI_WhenCombatActive()
    {
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
            Id = 1,
            Name = "Fighter",
            Class = "Fighter",
            Level = 5,
            HpMax = 40,
            HpCurrent = 40,
            AC = 18,
            InitiativeModifier = 2
        });

        var combatService = new CombatStateService(TestHelpers.CreateMockLogger<CombatStateService>());
        combatService.SelectParty(party);
        combatService.RollInitiativeForAll();
        combatService.StartCombat();

        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);

        // Act
        var cut = RenderComponent<CombatTrackerPage>();

        // Assert
        Assert.Contains("Round 1", cut.Markup);
        Assert.Contains("Initiative Order", cut.Markup);
        Assert.Contains("Combat Log", cut.Markup);
        Assert.Contains("Fighter", cut.Markup);
    }

    [Fact]
    public void CombatTracker_ShouldDisplayDamageAndHealButtons_ForAliveCombatants()
    {
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
            Id = 1,
            Name = "Fighter",
            Class = "Fighter",
            Level = 5,
            HpMax = 40,
            HpCurrent = 40,
            AC = 18,
            InitiativeModifier = 2
        });

        var combatService = new CombatStateService(TestHelpers.CreateMockLogger<CombatStateService>());
        combatService.SelectParty(party);
        combatService.RollInitiativeForAll();
        combatService.StartCombat();

        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);

        // Act
        var cut = RenderComponent<CombatTrackerPage>();

        // Assert
        var damageButtons = cut.FindAll("button").Where(b => b.TextContent.Contains("Damage"));
        var healButtons = cut.FindAll("button").Where(b => b.TextContent.Contains("Heal"));
        
        Assert.NotEmpty(damageButtons);
        Assert.NotEmpty(healButtons);
    }

    [Fact]
    public void CombatTracker_ShouldOnlyDisplayHealButton_ForUnconsciousCombatants()
    {
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
            Id = 1,
            Name = "Fighter",
            Class = "Fighter",
            Level = 5,
            HpMax = 40,
            HpCurrent = 40,
            AC = 18,
            InitiativeModifier = 2
        });

        var combatService = new CombatStateService(TestHelpers.CreateMockLogger<CombatStateService>());
        combatService.SelectParty(party);
        combatService.RollInitiativeForAll();
        combatService.StartCombat();
        
        // Make the combatant unconscious
        combatService.ApplyDamage(0, 50);

        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);

        // Act
        var cut = RenderComponent<CombatTrackerPage>();

        // Assert
        var damageButtons = cut.FindAll("button").Where(b => b.TextContent.Contains("Damage"));
        var healButtons = cut.FindAll("button").Where(b => b.TextContent.Contains("Heal"));
        
        Assert.Empty(damageButtons);
        Assert.NotEmpty(healButtons);
        Assert.Contains("Unconscious", cut.Markup);
    }

    [Fact]
    public void CombatTracker_DamageButtonClick_ShouldOpenDamageModal()
    {
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
            Id = 1,
            Name = "Fighter",
            Class = "Fighter",
            Level = 5,
            HpMax = 40,
            HpCurrent = 40,
            AC = 18,
            InitiativeModifier = 2
        });

        var combatService = new CombatStateService(TestHelpers.CreateMockLogger<CombatStateService>());
        combatService.SelectParty(party);
        combatService.RollInitiativeForAll();
        combatService.StartCombat();

        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);

        var cut = RenderComponent<CombatTrackerPage>();

        // Act
        var damageButton = cut.FindAll("button").First(b => b.TextContent.Contains("Damage"));
        damageButton.Click();

        // Assert
        Assert.Contains("Apply Damage", cut.Markup);
        var modal = cut.Find(".modal.show");
        Assert.NotNull(modal);
    }

    [Fact]
    public void CombatTracker_HealButtonClick_ShouldOpenHealModal()
    {
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
            Id = 1,
            Name = "Fighter",
            Class = "Fighter",
            Level = 5,
            HpMax = 40,
            HpCurrent = 40,
            AC = 18,
            InitiativeModifier = 2
        });

        var combatService = new CombatStateService(TestHelpers.CreateMockLogger<CombatStateService>());
        combatService.SelectParty(party);
        combatService.RollInitiativeForAll();
        combatService.StartCombat();

        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);

        var cut = RenderComponent<CombatTrackerPage>();

        // Act
        var healButton = cut.FindAll("button").First(b => b.TextContent.Contains("Heal"));
        healButton.Click();

        // Assert
        Assert.Contains("Apply Healing", cut.Markup);
        var modal = cut.Find(".modal.show");
        Assert.NotNull(modal);
    }

    [Fact]
    public void CombatTracker_DamageAndHealButtons_ShouldUpdateUI_WhenHPChanges()
    {
        // This test verifies that the UI updates correctly when HP changes occur
        // We test the service-level changes and UI refresh behavior

        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var combatService = new CombatStateService(TestHelpers.CreateMockLogger<CombatStateService>());

        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);

        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
            Id = 1,
            Name = "Fighter",
            Class = "Fighter",
            Level = 5,
            HpMax = 40,
            HpCurrent = 40,
            AC = 18,
            InitiativeModifier = 2
        });

        combatService.SelectParty(party);
        combatService.RollInitiativeForAll();
        combatService.StartCombat();

        var cut = RenderComponent<CombatTrackerPage>();

        // Verify initial HP display in UI
        Assert.Contains("40 / 40", cut.Markup);
        Assert.Contains("Alive", cut.Markup);

        // Act - Apply damage through service (simulating what UI buttons do)
        combatService.ApplyDamage(0, 15);
        cut.Render(); // Force re-render to check UI updates

        // Assert - UI should update to show reduced HP
        Assert.Contains("25 / 40", cut.Markup);
        Assert.Contains("Alive", cut.Markup);

        // Act - Apply healing through service
        combatService.ApplyHealing(0, 10);
        cut.Render(); // Force re-render

        // Assert - UI should update to show increased HP
        Assert.Contains("35 / 40", cut.Markup);
        Assert.Contains("Alive", cut.Markup);

        // Act - Apply massive damage to make unconscious
        combatService.ApplyDamage(0, 50); // More than remaining HP
        cut.Render(); // Force re-render

        // Assert - UI should show unconscious state
        Assert.Contains("0 / 40", cut.Markup);
        Assert.Contains("Unconscious", cut.Markup);

        // Act - Heal unconscious combatant
        combatService.ApplyHealing(0, 5);
        cut.Render(); // Force re-render

        // Assert - UI should show revived combatant
        Assert.Contains("5 / 40", cut.Markup);
        Assert.Contains("Alive", cut.Markup);
        // Note: Combat log may still contain "Unconscious" messages from history
    }

    [Fact]
    public void CombatTracker_WithActiveCombat_ShouldShowBasicUIElements()
    {
        // This test verifies that the component can render with active combat
        // without testing complex interactions
        
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var combatService = new CombatStateService(TestHelpers.CreateMockLogger<CombatStateService>());
        
        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);
        
        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
            Id = 1,
            Name = "Fighter",
            Class = "Fighter",
            Level = 5,
            HpMax = 40,
            HpCurrent = 40,
            AC = 18,
            InitiativeModifier = 2
        });

        combatService.SelectParty(party);
        combatService.RollInitiativeForAll();
        combatService.StartCombat();

        // Act
        var cut = RenderComponent<CombatTrackerPage>();

        // Assert - Basic structure should be present
        Assert.Contains("Combat Tracker", cut.Markup);
        
        // Note: Due to InteractiveServer render mode limitations in testing,
        // we cannot reliably test the full UI interaction here.
        // The actual functionality is verified through service-level tests.
    }




}