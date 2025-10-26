using Bunit;
using CombatTracker.Web.Components.Pages;
using CombatTracker.Web.Models;
using CombatTracker.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CombatTracker.Web.Tests.Pages;

/// <summary>
/// Tests for the Combat Setup page component
/// </summary>
public class CombatSetupPageTests : TestContext
{
    [Fact]
    public void CombatSetup_ShouldRenderTitle()
    {
        // Arrange
        Services.AddSingleton<PartyStateService>();
        Services.AddSingleton<CombatStateService>();

        // Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        var h1 = cut.Find("h1");
        Assert.Equal("Combat Setup", h1.TextContent);
    }

    [Fact]
    public void CombatSetup_ShouldDisplayNoPartiesWarning_WhenNoPartiesExist()
    {
        // Arrange
        Services.AddSingleton<PartyStateService>();
        Services.AddSingleton<CombatStateService>();

        // Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        var alert = cut.Find(".alert.alert-warning");
        Assert.Contains("No parties available", alert.TextContent);
    }

    [Fact]
    public void CombatSetup_ShouldDisplayPartySelector_WhenPartiesExist()
    {
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        partyService.CreateParty("Test Party");
        Services.AddSingleton(partyService);
        Services.AddSingleton<CombatStateService>();

        // Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        var select = cut.Find("#partySelect");
        Assert.NotNull(select);
        Assert.Contains("Test Party", cut.Markup);
    }

    [Fact]
    public void CombatSetup_ShouldRenderMonsterForm()
    {
        // Arrange
        Services.AddSingleton<PartyStateService>();
        Services.AddSingleton<CombatStateService>();

        // Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        Assert.NotNull(cut.Find("#monsterName"));
        Assert.NotNull(cut.Find("#monsterType"));
        Assert.NotNull(cut.Find("#monsterHp"));
        Assert.NotNull(cut.Find("#monsterAc"));
        Assert.NotNull(cut.Find("#monsterInitMod"));
    }

    [Fact]
    public void CombatSetup_ShouldDisplayCombatantsTable_WhenCombatantsExist()
    {
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
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

        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);

        // Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        var table = cut.Find("table");
        Assert.NotNull(table);
        Assert.Contains("Fighter", cut.Markup);
    }

    [Fact]
    public void CombatSetup_ShouldDisplayNoCombatantsMessage_WhenNoCombatants()
    {
        // Arrange
        Services.AddSingleton<PartyStateService>();
        Services.AddSingleton<CombatStateService>();

        // Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        Assert.Contains("No combatants added yet", cut.Markup);
    }

    [Fact]
    public void CombatSetup_ShouldRenderActionButtons()
    {
        // Arrange
        Services.AddSingleton<PartyStateService>();
        Services.AddSingleton<CombatStateService>();

        // Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        // Should show message when no combatants
        Assert.Contains("No combatants added yet", cut.Markup);
    }

    [Fact]
    public void CombatSetup_ShouldShowActionButtons_WhenCombatantsExist()
    {
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
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

        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);

        // Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        Assert.Contains("Roll Initiative", cut.Markup);
        Assert.Contains("Start Combat", cut.Markup);
        Assert.Contains("Reset", cut.Markup);
    }

    [Fact]
    public void CombatSetup_ShouldContainRollOptions_MonstersAndAll()
    {
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
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

        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);

        // Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        Assert.Contains("Monsters", cut.Markup);
        Assert.Contains("All", cut.Markup);
    }

    [Fact]
    public void CombatSetup_ClickMonsters_ShouldRollOnlyMonsters()
    {
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
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
        combatService.AddMonster(new Monster { Name = "Goblin", Type = "Humanoid", Hp = 7, AC = 15, InitiativeModifier = 2 });

        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);

        var cut = RenderComponent<CombatSetup>();

        // Ensure initial initiatives are 0
        Assert.Equal(0, combatService.Combatants["character-1"].Initiative);
        Assert.Equal(0, combatService.Combatants["monster-1"].Initiative);

        // Act - find the dropdown item with text Monsters and click it
        var monstersItem = cut.FindAll("a.dropdown-item").FirstOrDefault(x => x.TextContent.Contains("Monsters"));
        Assert.NotNull(monstersItem);
        monstersItem!.Click();

        // Assert - character should remain 0, monster should have initiative
        Assert.Equal(0, combatService.Combatants["character-1"].Initiative);
        Assert.NotEqual(0, combatService.Combatants["monster-1"].Initiative);
    }

    [Fact]
    public void CombatSetup_ClickAll_ShouldRollAllCombatants()
    {
        // Arrange
        var partyService = new PartyStateService(TestHelpers.CreateMockLogger<PartyStateService>());
        var party = partyService.CreateParty("Test Party");
        partyService.AddCharacter(party.Id, new Character
        {
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
        combatService.AddMonster(new Monster { Name = "Goblin", Type = "Humanoid", Hp = 7, AC = 15, InitiativeModifier = 2 });

        Services.AddSingleton(partyService);
        Services.AddSingleton(combatService);

        var cut = RenderComponent<CombatSetup>();

        // Ensure initial initiatives are 0
        Assert.Equal(0, combatService.Combatants["character-1"].Initiative);
        Assert.Equal(0, combatService.Combatants["monster-1"].Initiative);

        // Act - find the dropdown item with text All and click it
        var allItem = cut.FindAll("a.dropdown-item").FirstOrDefault(x => x.TextContent.Contains("All"));
        Assert.NotNull(allItem);
        allItem!.Click();

        // Assert - both character and monster should have initiative
        Assert.NotEqual(0, combatService.Combatants["character-1"].Initiative);
        Assert.NotEqual(0, combatService.Combatants["monster-1"].Initiative);
    }
}

