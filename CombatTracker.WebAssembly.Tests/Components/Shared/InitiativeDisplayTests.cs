using Bunit;
using CombatTracker.WebAssembly.Components.Shared;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Components.Shared;

/// <summary>
/// Tests for the InitiativeDisplay component
/// </summary>
public class InitiativeDisplayTests : TestContext
{
    [Fact]
    public void InitiativeDisplay_ShouldRenderInitiativeValue()
    {
        // Arrange
        var initiative = 15;

        // Act
        var cut = RenderComponent<InitiativeDisplay>(parameters => parameters
            .Add(p => p.Initiative, initiative));

        // Assert
        Assert.Contains("15", cut.Markup);
    }

    [Fact]
    public void InitiativeDisplay_ShouldRenderZeroInitiative()
    {
        // Arrange
        var initiative = 0;

        // Act
        var cut = RenderComponent<InitiativeDisplay>(parameters => parameters
            .Add(p => p.Initiative, initiative));

        // Assert
        Assert.Contains("0", cut.Markup);
    }

    [Fact]
    public void InitiativeDisplay_ShouldRenderNegativeInitiative()
    {
        // Arrange
        var initiative = -2;

        // Act
        var cut = RenderComponent<InitiativeDisplay>(parameters => parameters
            .Add(p => p.Initiative, initiative));

        // Assert
        Assert.Contains("-2", cut.Markup);
    }

    [Fact]
    public void InitiativeDisplay_ShouldHaveStrongTag()
    {
        // Arrange & Act
        var cut = RenderComponent<InitiativeDisplay>(parameters => parameters
            .Add(p => p.Initiative, 10));

        // Assert
        var strong = cut.Find("strong");
        Assert.NotNull(strong);
        Assert.Contains("10", strong.TextContent);
    }

    [Fact]
    public void InitiativeDisplay_ShouldHaveCorrectFontSize()
    {
        // Arrange & Act
        var cut = RenderComponent<InitiativeDisplay>(parameters => parameters
            .Add(p => p.Initiative, 10));

        // Assert
        var strong = cut.Find("strong");
        Assert.Contains("font-size: 1.1rem", strong.GetAttribute("style"));
    }
}
