using Bunit;
using CombatTracker.WebAssembly.Components.CombatTrackerFolder;
using CombatTracker.WebAssembly.Models;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Components.CombatTracker;

/// <summary>
/// Tests for the CombatLog component
/// </summary>
public class CombatLogTests : TestContext
{
    [Fact]
    public void CombatLog_ShouldDisplayEmptyMessage_WhenNoEntries()
    {
        // Arrange
        var emptyLog = new List<CombatLogEntry>();

        // Act
        var cut = RenderComponent<CombatLog>(parameters => parameters
            .Add(p => p.LogEntries, emptyLog));

        // Assert
        Assert.Contains("No entries yet", cut.Markup);
    }

    [Fact]
    public void CombatLog_ShouldDisplayLogEntries_WhenEntriesExist()
    {
        // Arrange
        var logEntries = new List<CombatLogEntry>
        {
            new CombatLogEntry
            {
                Round = 1,
                Type = "Turn",
                Message = "Fighter's turn",
                Timestamp = DateTime.Now
            }
        };

        // Act
        var cut = RenderComponent<CombatLog>(parameters => parameters
            .Add(p => p.LogEntries, logEntries));

        // Assert
        Assert.Contains("Fighter's turn", cut.Markup);
        Assert.Contains("Round 1", cut.Markup);
    }

    [Fact]
    public void CombatLog_ShouldDisplayTurnBadge_ForTurnEntries()
    {
        // Arrange
        var logEntries = new List<CombatLogEntry>
        {
            new CombatLogEntry
            {
                Round = 1,
                Type = "Turn",
                Message = "Fighter's turn",
                Timestamp = DateTime.Now
            }
        };

        // Act
        var cut = RenderComponent<CombatLog>(parameters => parameters
            .Add(p => p.LogEntries, logEntries));

        // Assert
        var badge = cut.Find(".badge.bg-primary");
        Assert.NotNull(badge);
        Assert.Contains("Turn", badge.TextContent);
    }

    [Fact]
    public void CombatLog_ShouldDisplayDamageBadge_ForDamageEntries()
    {
        // Arrange
        var logEntries = new List<CombatLogEntry>
        {
            new CombatLogEntry
            {
                Round = 1,
                Type = "Damage",
                Message = "Fighter takes 10 damage",
                Timestamp = DateTime.Now
            }
        };

        // Act
        var cut = RenderComponent<CombatLog>(parameters => parameters
            .Add(p => p.LogEntries, logEntries));

        // Assert
        var badge = cut.Find(".badge.bg-danger");
        Assert.NotNull(badge);
        Assert.Contains("Damage", badge.TextContent);
    }

    [Fact]
    public void CombatLog_ShouldDisplayHealBadge_ForHealEntries()
    {
        // Arrange
        var logEntries = new List<CombatLogEntry>
        {
            new CombatLogEntry
            {
                Round = 1,
                Type = "Heal",
                Message = "Fighter heals 15 HP",
                Timestamp = DateTime.Now
            }
        };

        // Act
        var cut = RenderComponent<CombatLog>(parameters => parameters
            .Add(p => p.LogEntries, logEntries));

        // Assert
        var badge = cut.Find(".badge.bg-success");
        Assert.NotNull(badge);
        Assert.Contains("Heal", badge.TextContent);
    }

    [Fact]
    public void CombatLog_ShouldDisplayStatusBadge_ForStatusEntries()
    {
        // Arrange
        var logEntries = new List<CombatLogEntry>
        {
            new CombatLogEntry
            {
                Round = 1,
                Type = "Status",
                Message = "Fighter is unconscious",
                Timestamp = DateTime.Now
            }
        };

        // Act
        var cut = RenderComponent<CombatLog>(parameters => parameters
            .Add(p => p.LogEntries, logEntries));

        // Assert
        var badge = cut.Find(".badge.bg-warning");
        Assert.NotNull(badge);
        Assert.Contains("Status", badge.TextContent);
    }

    [Fact]
    public void CombatLog_ShouldDisplayMultipleEntries()
    {
        // Arrange
        var logEntries = new List<CombatLogEntry>
        {
            new CombatLogEntry
            {
                Round = 1,
                Type = "Turn",
                Message = "Fighter's turn",
                Timestamp = DateTime.Now
            },
            new CombatLogEntry
            {
                Round = 1,
                Type = "Damage",
                Message = "Goblin takes 10 damage",
                Timestamp = DateTime.Now.AddSeconds(5)
            }
        };

        // Act
        var cut = RenderComponent<CombatLog>(parameters => parameters
            .Add(p => p.LogEntries, logEntries));

        // Assert
        Assert.Contains("Fighter's turn", cut.Markup);
        Assert.Contains("Goblin takes 10 damage", cut.Markup);
        
        var entries = cut.FindAll(".combat-log-entry");
        Assert.Equal(2, entries.Count);
    }

    [Fact]
    public void CombatLog_ShouldDisplayEntriesInReverseOrder()
    {
        // Arrange
        var logEntries = new List<CombatLogEntry>
        {
            new CombatLogEntry
            {
                Round = 1,
                Type = "Turn",
                Message = "First entry",
                Timestamp = DateTime.Now.AddSeconds(-10)
            },
            new CombatLogEntry
            {
                Round = 1,
                Type = "Damage",
                Message = "Second entry",
                Timestamp = DateTime.Now
            }
        };

        // Act
        var cut = RenderComponent<CombatLog>(parameters => parameters
            .Add(p => p.LogEntries, logEntries));

        // Assert - Newest first (reverse order)
        var entries = cut.FindAll(".combat-log-entry");
        Assert.Contains("Second entry", entries[0].TextContent);
        Assert.Contains("First entry", entries[1].TextContent);
    }

    [Fact]
    public void CombatLog_ShouldDisplayTimestamp()
    {
        // Arrange
        var timestamp = DateTime.Now;
        var logEntries = new List<CombatLogEntry>
        {
            new CombatLogEntry
            {
                Round = 1,
                Type = "Turn",
                Message = "Test entry",
                Timestamp = timestamp
            }
        };

        // Act
        var cut = RenderComponent<CombatLog>(parameters => parameters
            .Add(p => p.LogEntries, logEntries));

        // Assert
        Assert.Contains(timestamp.ToString("HH:mm:ss"), cut.Markup);
    }

    [Fact]
    public void CombatLog_ShouldHaveCardStructure()
    {
        // Arrange
        var logEntries = new List<CombatLogEntry>();

        // Act
        var cut = RenderComponent<CombatLog>(parameters => parameters
            .Add(p => p.LogEntries, logEntries));

        // Assert
        var card = cut.Find(".card");
        Assert.NotNull(card);
        
        var cardHeader = cut.Find(".card-header");
        Assert.NotNull(cardHeader);
        Assert.Contains("Combat Log", cardHeader.TextContent);
        
        var cardBody = cut.Find(".card-body");
        Assert.NotNull(cardBody);
    }
}
