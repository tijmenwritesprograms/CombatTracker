using Bunit;
using CombatTracker.Web.Components.Pages;
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
        // Arrange & Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        var h1 = cut.Find("h1");
        Assert.Equal("Combat Setup", h1.TextContent);
    }

    [Fact]
    public void CombatSetup_ShouldHavePageTitleComponent()
    {
        // Arrange & Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        var markup = cut.Markup;
        Assert.Contains("Combat Setup", markup);
    }

    [Fact]
    public void CombatSetup_ShouldRenderComingSoonAlert()
    {
        // Arrange & Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        var alert = cut.Find(".alert.alert-info");
        Assert.Contains("Coming Soon", alert.TextContent);
    }

    [Fact]
    public void CombatSetup_ShouldRenderBackToHomeLink()
    {
        // Arrange & Act
        var cut = RenderComponent<CombatSetup>();

        // Assert
        var link = cut.Find("a[href='/']");
        Assert.NotNull(link);
        Assert.Equal("Back to Home", link.TextContent);
    }
}
