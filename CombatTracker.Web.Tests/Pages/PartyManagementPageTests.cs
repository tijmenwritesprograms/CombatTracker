using Bunit;
using CombatTracker.Web.Components.Pages;
using Xunit;

namespace CombatTracker.Web.Tests.Pages;

/// <summary>
/// Tests for the Party Management page component
/// </summary>
public class PartyManagementPageTests : TestContext
{
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
    public void PartyManagement_ShouldRenderComingSoonAlert()
    {
        // Arrange & Act
        var cut = RenderComponent<PartyManagement>();

        // Assert
        var alert = cut.Find(".alert.alert-info");
        Assert.Contains("Coming Soon", alert.TextContent);
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
}
