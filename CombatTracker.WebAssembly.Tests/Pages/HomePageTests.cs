using Bunit;
using CombatTracker.WebAssembly.Components.Pages;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Pages;

/// <summary>
/// Tests for the Home page component
/// </summary>
public class HomePageTests : TestContext
{
    [Fact]
    public void Home_ShouldRenderTitle()
    {
        // Arrange & Act
        var cut = RenderComponent<Home>();

        // Assert
        var h1 = cut.Find("h1");
        Assert.Equal("D&D Combat Tracker", h1.TextContent);
    }

    [Fact]
    public void Home_ShouldHavePageTitleComponent()
    {
        // Arrange & Act
        var cut = RenderComponent<Home>();

        // Assert
        var markup = cut.Markup;
        Assert.Contains("D&D Combat Tracker", markup);
    }

    [Fact]
    public void Home_ShouldRenderWelcomeMessage()
    {
        // Arrange & Act
        var cut = RenderComponent<Home>();

        // Assert
        var leadParagraph = cut.Find("p.lead");
        Assert.Contains("Welcome to your combat management companion", leadParagraph.TextContent);
    }

    [Fact]
    public void Home_ShouldRenderPartyManagementCard()
    {
        // Arrange & Act
        var cut = RenderComponent<Home>();

        // Assert
        var cardTitle = cut.Find("h5.card-title");
        Assert.Contains("Party Management", cardTitle.TextContent);
        
        var link = cut.Find("a[href='party-management']");
        Assert.NotNull(link);
        Assert.Equal("Manage Party", link.TextContent);
    }

    [Fact]
    public void Home_ShouldRenderCombatSetupCard()
    {
        // Arrange & Act
        var cut = RenderComponent<Home>();

        // Assert
        var cards = cut.FindAll("h5.card-title");
        Assert.Contains(cards, card => card.TextContent.Contains("Combat Setup"));
        
        var link = cut.Find("a[href='combat-setup']");
        Assert.NotNull(link);
        Assert.Equal("Setup Combat", link.TextContent);
    }

    [Fact]
    public void Home_ShouldRenderThreeCards()
    {
        // Arrange & Act
        var cut = RenderComponent<Home>();

        // Assert
        var cards = cut.FindAll(".card");
        Assert.Equal(3, cards.Count);
    }

    [Fact]
    public void Home_ShouldUseBootstrapGridLayout()
    {
        // Arrange & Act
        var cut = RenderComponent<Home>();

        // Assert
        var row = cut.Find(".row");
        Assert.NotNull(row);
        
        var columns = cut.FindAll(".col-md-4");
        Assert.Equal(3, columns.Count);
    }
}
