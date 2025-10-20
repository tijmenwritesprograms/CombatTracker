using Bunit;
using CombatTracker.Web.Components.Pages;
using CombatTracker.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CombatTracker.Web.Tests.Pages;

/// <summary>
/// Tests for the Party Management page component
/// </summary>
public class PartyManagementPageTests : TestContext
{
    public PartyManagementPageTests()
    {
        // Register the PartyStateService for all tests
        Services.AddSingleton<PartyStateService>();
    }

    [Fact]
    public void PartyManagement_ShouldRenderTitle()
    {
        // Arrange & Act
        var cut = RenderComponent<PartyManagement>();

        // Assert
        var h1 = cut.Find("h1");
        Assert.Equal("Party Management", h1.TextContent);
    }

    [Fact]
    public void PartyManagement_ShouldHavePageTitleComponent()
    {
        // Arrange & Act
        var cut = RenderComponent<PartyManagement>();

        // Assert
        var markup = cut.Markup;
        Assert.Contains("Party Management", markup);
    }

    [Fact]
    public void PartyManagement_ShouldRenderCreateNewPartyButton()
    {
        // Arrange & Act
        var cut = RenderComponent<PartyManagement>();

        // Assert
        var button = cut.Find("button");
        Assert.Contains("Create New Party", button.TextContent);
    }

    [Fact]
    public void PartyManagement_ShouldRenderBackToHomeLink()
    {
        // Arrange & Act
        var cut = RenderComponent<PartyManagement>();

        // Assert
        var link = cut.Find("a[href='/']");
        Assert.NotNull(link);
        Assert.Equal("Back to Home", link.TextContent);
    }

    [Fact]
    public void PartyManagement_ShouldShowNoPartiesMessage_WhenNoPartiesExist()
    {
        // Arrange & Act
        var cut = RenderComponent<PartyManagement>();

        // Assert
        var alert = cut.Find(".alert.alert-info");
        Assert.Contains("No parties created yet", alert.TextContent);
    }

    [Fact]
    public void PartyManagement_ShouldShowPartyForm_WhenCreateButtonClicked()
    {
        // Arrange
        var cut = RenderComponent<PartyManagement>();
        var createButton = cut.Find("button");

        // Act
        createButton.Click();

        // Assert
        var form = cut.Find(".card");
        Assert.Contains("Create New Party", form.TextContent);
    }

    [Fact]
    public void PartyManagement_ShouldDisplayParty_AfterCreation()
    {
        // Arrange
        var partyService = Services.GetService<PartyStateService>();
        partyService!.CreateParty("Test Party");

        // Act
        var cut = RenderComponent<PartyManagement>();

        // Assert
        var cards = cut.FindAll(".card");
        Assert.True(cards.Count > 0);
        Assert.Contains("Test Party", cut.Markup);
    }
}
