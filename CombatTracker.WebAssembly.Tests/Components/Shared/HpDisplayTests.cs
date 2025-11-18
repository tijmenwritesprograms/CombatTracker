using Bunit;
using CombatTracker.WebAssembly.Components.Shared;
using CombatTracker.WebAssembly.Models;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Components.Shared;

/// <summary>
/// Tests for the HpDisplay component
/// </summary>
public class HpDisplayTests : TestContext
{
    [Fact]
    public void HpDisplay_ShouldDisplayCurrentAndMaxHp()
    {
        // Arrange & Act
        var cut = RenderComponent<HpDisplay>(parameters => parameters
            .Add(p => p.HpCurrent, 25)
            .Add(p => p.HpMax, 40)
            .Add(p => p.Status, Models.Status.Alive));

        // Assert
        Assert.Contains("25 / 40", cut.Markup);
    }

    [Fact]
    public void HpDisplay_ShouldShow0HpBadge_WhenHpIsZero()
    {
        // Arrange & Act
        var cut = RenderComponent<HpDisplay>(parameters => parameters
            .Add(p => p.HpCurrent, 0)
            .Add(p => p.HpMax, 40)
            .Add(p => p.Status, Models.Status.Unconscious));

        // Assert
        Assert.Contains("0 HP", cut.Markup);
        var badge = cut.Find(".badge.bg-danger");
        Assert.NotNull(badge);
    }

    [Fact]
    public void HpDisplay_ShouldShowBloodiedBadge_WhenHpBelowHalf()
    {
        // Arrange & Act
        var cut = RenderComponent<HpDisplay>(parameters => parameters
            .Add(p => p.HpCurrent, 15)
            .Add(p => p.HpMax, 40)
            .Add(p => p.Status, Models.Status.Alive));

        // Assert
        Assert.Contains("Bloodied", cut.Markup);
        var badge = cut.Find(".badge.bg-warning");
        Assert.NotNull(badge);
    }

    [Fact]
    public void HpDisplay_ShouldNotShowBadge_WhenHpAboveHalf()
    {
        // Arrange & Act
        var cut = RenderComponent<HpDisplay>(parameters => parameters
            .Add(p => p.HpCurrent, 25)
            .Add(p => p.HpMax, 40)
            .Add(p => p.Status, Models.Status.Alive));

        // Assert
        var badges = cut.FindAll(".badge");
        Assert.Empty(badges);
    }

    [Fact]
    public void HpDisplay_ShouldShowHpBar_WhenHpAboveZero()
    {
        // Arrange & Act
        var cut = RenderComponent<HpDisplay>(parameters => parameters
            .Add(p => p.HpCurrent, 25)
            .Add(p => p.HpMax, 40)
            .Add(p => p.Status, Models.Status.Alive));

        // Assert
        var hpBar = cut.Find(".hp-bar");
        Assert.NotNull(hpBar);
        
        var hpBarFill = cut.Find(".hp-bar-fill");
        Assert.NotNull(hpBarFill);
    }

    [Fact]
    public void HpDisplay_ShouldNotShowHpBar_WhenHpIsZero()
    {
        // Arrange & Act
        var cut = RenderComponent<HpDisplay>(parameters => parameters
            .Add(p => p.HpCurrent, 0)
            .Add(p => p.HpMax, 40)
            .Add(p => p.Status, Models.Status.Unconscious));

        // Assert
        var hpBars = cut.FindAll(".hp-bar");
        Assert.Empty(hpBars);
    }

    [Fact]
    public void HpDisplay_ShouldCalculateCorrectHpPercentage()
    {
        // Arrange & Act
        var cut = RenderComponent<HpDisplay>(parameters => parameters
            .Add(p => p.HpCurrent, 30)
            .Add(p => p.HpMax, 40)
            .Add(p => p.Status, Models.Status.Alive));

        // Assert - 30/40 = 75%
        var hpBarFill = cut.Find(".hp-bar-fill");
        Assert.Contains("width: 75%", hpBarFill.GetAttribute("style"));
    }

    [Fact]
    public void HpDisplay_ShouldStrikeThrough_WhenNotAlive()
    {
        // Arrange & Act
        var cut = RenderComponent<HpDisplay>(parameters => parameters
            .Add(p => p.HpCurrent, 0)
            .Add(p => p.HpMax, 40)
            .Add(p => p.Status, Models.Status.Dead));

        // Assert
        var span = cut.Find("span");
        Assert.Contains("text-decoration-line-through", span.ClassName);
        Assert.Contains("text-muted", span.ClassName);
    }

    [Fact]
    public void HpDisplay_ShouldNotStrikeThrough_WhenAlive()
    {
        // Arrange & Act
        var cut = RenderComponent<HpDisplay>(parameters => parameters
            .Add(p => p.HpCurrent, 40)
            .Add(p => p.HpMax, 40)
            .Add(p => p.Status, Models.Status.Alive));

        // Assert
        var span = cut.Find("span");
        Assert.DoesNotContain("text-decoration-line-through", span.ClassName ?? "");
    }

    [Fact]
    public void HpDisplay_ShouldShowHpBarTitle()
    {
        // Arrange & Act
        var cut = RenderComponent<HpDisplay>(parameters => parameters
            .Add(p => p.HpCurrent, 25)
            .Add(p => p.HpMax, 40)
            .Add(p => p.Status, Models.Status.Alive));

        // Assert
        var hpBar = cut.Find(".hp-bar");
        Assert.Equal("HP: 25 / 40", hpBar.GetAttribute("title"));
    }
}
