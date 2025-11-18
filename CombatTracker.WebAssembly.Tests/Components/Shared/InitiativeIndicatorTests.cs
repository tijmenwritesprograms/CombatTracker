using Bunit;
using CombatTracker.WebAssembly.Components.Shared;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Components.Shared;

/// <summary>
/// Tests for the InitiativeIndicator component
/// </summary>
public class InitiativeIndicatorTests : TestContext
{
    [Fact]
    public void InitiativeIndicator_ShouldNotRender_WhenIsCurrentTurnIsFalse()
    {
        // Arrange & Act
        var cut = RenderComponent<InitiativeIndicator>(parameters => parameters
            .Add(p => p.IsCurrentTurn, false));

        // Assert
        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void InitiativeIndicator_ShouldRender_WhenIsCurrentTurnIsTrue()
    {
        // Arrange & Act
        var cut = RenderComponent<InitiativeIndicator>(parameters => parameters
            .Add(p => p.IsCurrentTurn, true));

        // Assert
        var indicator = cut.Find(".initiative-indicator");
        Assert.NotNull(indicator);
    }

    [Fact]
    public void InitiativeIndicator_ShouldHaveCorrectTitle_WhenRendered()
    {
        // Arrange & Act
        var cut = RenderComponent<InitiativeIndicator>(parameters => parameters
            .Add(p => p.IsCurrentTurn, true));

        // Assert
        var indicator = cut.Find(".initiative-indicator");
        Assert.Equal("Current Turn", indicator.GetAttribute("title"));
    }

    [Fact]
    public void InitiativeIndicator_ShouldHaveAriaLabel_WhenRendered()
    {
        // Arrange & Act
        var cut = RenderComponent<InitiativeIndicator>(parameters => parameters
            .Add(p => p.IsCurrentTurn, true));

        // Assert
        var indicator = cut.Find(".initiative-indicator");
        Assert.Equal("Current turn indicator", indicator.GetAttribute("aria-label"));
    }
}
