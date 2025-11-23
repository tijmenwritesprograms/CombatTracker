using Bunit;
using CombatTracker.WebAssembly.Components.Shared;
using CombatTracker.WebAssembly.Models;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Components.Shared;

/// <summary>
/// Tests for the StatusBadge component
/// </summary>
public class StatusBadgeTests : TestContext
{
    [Fact]
    public void StatusBadge_ShouldShowAliveBadge_WhenStatusIsAlive()
    {
        // Arrange & Act
        var cut = RenderComponent<StatusBadge>(parameters => parameters
            .Add(p => p.Status, Models.Status.Alive));

        // Assert
        Assert.Contains("Alive", cut.Markup);
        var badge = cut.Find(".badge.bg-success");
        Assert.NotNull(badge);
    }

    [Fact]
    public void StatusBadge_ShouldShowUnconsciousBadge_WhenStatusIsUnconscious()
    {
        // Arrange & Act
        var cut = RenderComponent<StatusBadge>(parameters => parameters
            .Add(p => p.Status, Models.Status.Unconscious));

        // Assert
        Assert.Contains("Unconscious", cut.Markup);
        var badge = cut.Find(".badge.bg-danger");
        Assert.NotNull(badge);
    }

    [Fact]
    public void StatusBadge_ShouldShowDeadBadge_WhenStatusIsDead()
    {
        // Arrange & Act
        var cut = RenderComponent<StatusBadge>(parameters => parameters
            .Add(p => p.Status, Models.Status.Dead));

        // Assert
        Assert.Contains("Dead", cut.Markup);
        var badge = cut.Find(".badge.bg-dark");
        Assert.NotNull(badge);
    }

    [Fact]
    public void StatusBadge_ShouldOnlyShowOneBadge()
    {
        // Arrange & Act
        var cut = RenderComponent<StatusBadge>(parameters => parameters
            .Add(p => p.Status, Models.Status.Alive));

        // Assert
        var badges = cut.FindAll(".badge");
        Assert.Single(badges);
    }
}
