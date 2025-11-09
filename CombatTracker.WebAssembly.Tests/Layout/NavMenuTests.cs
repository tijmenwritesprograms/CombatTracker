using Bunit;
using CombatTracker.WebAssembly.Components.Layout;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Layout;

/// <summary>
/// Tests for the NavMenu component
/// </summary>
public class NavMenuTests : TestContext
{
    public NavMenuTests()
    {
        // Register NavigationManager service required by NavLink
        Services.AddSingleton<NavigationManager>(new MockNavigationManager());
    }

    [Fact]
    public void NavMenu_ShouldRenderBrandName()
    {
        // Arrange & Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        var brand = cut.Find(".navbar-brand");
        Assert.Equal("D&D Combat Tracker", brand.TextContent);
    }

    [Fact]
    public void NavMenu_ShouldRenderHomeLink()
    {
        // Arrange & Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        var links = cut.FindAll(".nav-link");
        var homeLink = links.FirstOrDefault(l => l.TextContent.Contains("Home"));
        Assert.NotNull(homeLink);
    }

    [Fact]
    public void NavMenu_ShouldRenderPartyManagementLink()
    {
        // Arrange & Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        var links = cut.FindAll(".nav-link");
        var partyLink = links.FirstOrDefault(l => l.TextContent.Contains("Party Management"));
        Assert.NotNull(partyLink);
    }

    [Fact]
    public void NavMenu_ShouldRenderCombatSetupLink()
    {
        // Arrange & Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        var links = cut.FindAll(".nav-link");
        var combatLink = links.FirstOrDefault(l => l.TextContent.Contains("Combat Setup"));
        Assert.NotNull(combatLink);
    }

    [Fact]
    public void NavMenu_ShouldRenderSixNavigationItems()
    {
        // Arrange & Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        var navItems = cut.FindAll(".nav-item");
        Assert.Equal(6, navItems.Count);
    }

    [Fact]
    public void NavMenu_ShouldRenderCombatTrackerLink()
    {
        // Arrange & Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        var links = cut.FindAll(".nav-link");
        var trackerLink = links.FirstOrDefault(l => l.TextContent.Contains("Combat Tracker"));
        Assert.NotNull(trackerLink);
    }

    [Fact]
    public void NavMenu_ShouldRenderSettingsLink()
    {
        // Arrange & Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        var links = cut.FindAll(".nav-link");
        var settingsLink = links.FirstOrDefault(l => l.TextContent.Contains("Settings"));
        Assert.NotNull(settingsLink);
    }

    [Fact]
    public void NavMenu_ShouldHaveToggler()
    {
        // Arrange & Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        var toggler = cut.Find(".navbar-toggler");
        Assert.NotNull(toggler);
    }

    [Fact]
    public void NavMenu_ShouldHaveNavScrollableSection()
    {
        // Arrange & Act
        var cut = RenderComponent<NavMenu>();

        // Assert
        var scrollable = cut.Find(".nav-scrollable");
        Assert.NotNull(scrollable);
    }
}

/// <summary>
/// Mock NavigationManager for testing
/// </summary>
internal class MockNavigationManager : NavigationManager
{
    public MockNavigationManager()
    {
        Initialize("http://localhost/", "http://localhost/");
    }

    protected override void NavigateToCore(string uri, bool forceLoad)
    {
        // Do nothing for tests
    }
}
