using Bunit;
using CombatTracker.WebAssembly.Components.Shared;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Components.Shared;

/// <summary>
/// Tests for the MonsterNameDisplay component
/// </summary>
public class MonsterNameDisplayTests : TestContext
{
    [Fact]
    public void MonsterNameDisplay_ShouldDisplayName()
    {
        // Arrange
        var name = "Orc Warrior";

        // Act
        var cut = RenderComponent<MonsterNameDisplay>(parameters => parameters
            .Add(p => p.Name, name)
            .Add(p => p.ReferenceId, 1)
            .Add(p => p.CombatantIndex, 0)
            .Add(p => p.IsExpanded, false)
            .Add(p => p.OnShowStatblock, EventCallback.Factory.Create<int>(this, _ => { }))
            .Add(p => p.OnToggleQuickReference, EventCallback.Factory.Create<int>(this, _ => { })));

        // Assert
        Assert.Contains("Orc Warrior", cut.Markup);
    }

    [Fact]
    public void MonsterNameDisplay_ShouldHaveClickableNameLink()
    {
        // Arrange & Act
        var cut = RenderComponent<MonsterNameDisplay>(parameters => parameters
            .Add(p => p.Name, "Goblin")
            .Add(p => p.ReferenceId, 1)
            .Add(p => p.CombatantIndex, 0)
            .Add(p => p.IsExpanded, false)
            .Add(p => p.OnShowStatblock, EventCallback.Factory.Create<int>(this, _ => { }))
            .Add(p => p.OnToggleQuickReference, EventCallback.Factory.Create<int>(this, _ => { })));

        // Assert
        var nameLink = cut.Find(".monster-name-link");
        Assert.NotNull(nameLink);
        Assert.Equal("Goblin", nameLink.TextContent);
    }

    [Fact]
    public void MonsterNameDisplay_ShouldHaveToggleButton()
    {
        // Arrange & Act
        var cut = RenderComponent<MonsterNameDisplay>(parameters => parameters
            .Add(p => p.Name, "Goblin")
            .Add(p => p.ReferenceId, 1)
            .Add(p => p.CombatantIndex, 0)
            .Add(p => p.IsExpanded, false)
            .Add(p => p.OnShowStatblock, EventCallback.Factory.Create<int>(this, _ => { }))
            .Add(p => p.OnToggleQuickReference, EventCallback.Factory.Create<int>(this, _ => { })));

        // Assert
        var button = cut.Find("button");
        Assert.NotNull(button);
    }

    [Fact]
    public void MonsterNameDisplay_ShouldShowChevronDown_WhenNotExpanded()
    {
        // Arrange & Act
        var cut = RenderComponent<MonsterNameDisplay>(parameters => parameters
            .Add(p => p.Name, "Goblin")
            .Add(p => p.ReferenceId, 1)
            .Add(p => p.CombatantIndex, 0)
            .Add(p => p.IsExpanded, false)
            .Add(p => p.OnShowStatblock, EventCallback.Factory.Create<int>(this, _ => { }))
            .Add(p => p.OnToggleQuickReference, EventCallback.Factory.Create<int>(this, _ => { })));

        // Assert
        var icon = cut.Find(".bi-chevron-down");
        Assert.NotNull(icon);
    }

    [Fact]
    public void MonsterNameDisplay_ShouldShowChevronUp_WhenExpanded()
    {
        // Arrange & Act
        var cut = RenderComponent<MonsterNameDisplay>(parameters => parameters
            .Add(p => p.Name, "Goblin")
            .Add(p => p.ReferenceId, 1)
            .Add(p => p.CombatantIndex, 0)
            .Add(p => p.IsExpanded, true)
            .Add(p => p.OnShowStatblock, EventCallback.Factory.Create<int>(this, _ => { }))
            .Add(p => p.OnToggleQuickReference, EventCallback.Factory.Create<int>(this, _ => { })));

        // Assert
        var icon = cut.Find(".bi-chevron-up");
        Assert.NotNull(icon);
    }

    [Fact]
    public void MonsterNameDisplay_ShouldCallOnShowStatblock_WhenNameClicked()
    {
        // Arrange
        var statblockCalled = false;
        var referenceId = 5;

        var cut = RenderComponent<MonsterNameDisplay>(parameters => parameters
            .Add(p => p.Name, "Goblin")
            .Add(p => p.ReferenceId, referenceId)
            .Add(p => p.CombatantIndex, 0)
            .Add(p => p.IsExpanded, false)
            .Add(p => p.OnShowStatblock, EventCallback.Factory.Create<int>(this, id =>
            {
                statblockCalled = true;
                Assert.Equal(referenceId, id);
            }))
            .Add(p => p.OnToggleQuickReference, EventCallback.Factory.Create<int>(this, _ => { })));

        // Act
        var nameLink = cut.Find(".monster-name-link");
        nameLink.Click();

        // Assert
        Assert.True(statblockCalled);
    }

    [Fact]
    public void MonsterNameDisplay_ShouldCallOnToggleQuickReference_WhenButtonClicked()
    {
        // Arrange
        var toggleCalled = false;
        var combatantIndex = 3;

        var cut = RenderComponent<MonsterNameDisplay>(parameters => parameters
            .Add(p => p.Name, "Goblin")
            .Add(p => p.ReferenceId, 1)
            .Add(p => p.CombatantIndex, combatantIndex)
            .Add(p => p.IsExpanded, false)
            .Add(p => p.OnShowStatblock, EventCallback.Factory.Create<int>(this, _ => { }))
            .Add(p => p.OnToggleQuickReference, EventCallback.Factory.Create<int>(this, index =>
            {
                toggleCalled = true;
                Assert.Equal(combatantIndex, index);
            })));

        // Act
        var button = cut.Find("button");
        button.Click();

        // Assert
        Assert.True(toggleCalled);
    }

    [Fact]
    public void MonsterNameDisplay_ShouldHaveAccessibilityAttributes()
    {
        // Arrange & Act
        var cut = RenderComponent<MonsterNameDisplay>(parameters => parameters
            .Add(p => p.Name, "Goblin")
            .Add(p => p.ReferenceId, 1)
            .Add(p => p.CombatantIndex, 0)
            .Add(p => p.IsExpanded, false)
            .Add(p => p.OnShowStatblock, EventCallback.Factory.Create<int>(this, _ => { }))
            .Add(p => p.OnToggleQuickReference, EventCallback.Factory.Create<int>(this, _ => { })));

        // Assert
        var button = cut.Find("button");
        Assert.Equal("Toggle quick reference", button.GetAttribute("title"));
        Assert.Equal("Toggle quick reference", button.GetAttribute("aria-label"));
    }
}
