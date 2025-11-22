using Bunit;
using CombatTracker.WebAssembly.Components.Shared;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Components.Shared;

/// <summary>
/// Tests for the ArmorClassDisplay component
/// </summary>
public class ArmorClassDisplayTests : TestContext
{
    [Fact]
    public void ArmorClassDisplay_ShouldDisplayArmorClass()
    {
        // Arrange
        var ac = 18;

        // Act
        var cut = RenderComponent<ArmorClassDisplay>(parameters => parameters
            .Add(p => p.ArmorClass, ac));

        // Assert
        Assert.Contains("AC 18", cut.Markup);
    }

    [Fact]
    public void ArmorClassDisplay_ShouldHaveBadge()
    {
        // Arrange & Act
        var cut = RenderComponent<ArmorClassDisplay>(parameters => parameters
            .Add(p => p.ArmorClass, 15));

        // Assert
        var badge = cut.Find(".badge.bg-secondary");
        Assert.NotNull(badge);
    }

    [Fact]
    public void ArmorClassDisplay_ShouldDisplayZeroAC()
    {
        // Arrange & Act
        var cut = RenderComponent<ArmorClassDisplay>(parameters => parameters
            .Add(p => p.ArmorClass, 0));

        // Assert
        Assert.Contains("AC 0", cut.Markup);
    }

    [Fact]
    public void ArmorClassDisplay_ShouldDisplayHighAC()
    {
        // Arrange & Act
        var cut = RenderComponent<ArmorClassDisplay>(parameters => parameters
            .Add(p => p.ArmorClass, 25));

        // Assert
        Assert.Contains("AC 25", cut.Markup);
    }

    [Fact]
    public void ArmorClassDisplay_ShouldDisplayNegativeAC()
    {
        // Arrange & Act
        var cut = RenderComponent<ArmorClassDisplay>(parameters => parameters
            .Add(p => p.ArmorClass, -5));

        // Assert
        Assert.Contains("AC -5", cut.Markup);
    }
}
