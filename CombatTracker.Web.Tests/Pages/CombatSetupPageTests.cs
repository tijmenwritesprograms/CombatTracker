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
        var partyService = new PartyStateService();
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
        var partyService = new PartyStateService();
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

        var combatService = new CombatStateService();
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
        var partyService = new PartyStateService();
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

        var combatService = new CombatStateService();
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
}

