using Bunit;
using CombatTracker.WebAssembly.Components.CombatTrackerFolder;
using Microsoft.AspNetCore.Components;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Components.CombatTracker;

/// <summary>
/// Tests for the DamageModal component
/// </summary>
public class DamageModalTests : TestContext
{
    [Fact]
    public void DamageModal_ShouldNotRender_WhenNotVisible()
    {
        // Arrange & Act
        var cut = RenderComponent<DamageModal>(parameters => parameters
            .Add(p => p.IsVisible, false)
            .Add(p => p.Amount, 5)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { }))
            .Add(p => p.OnApply, EventCallback.Factory.Create(this, () => { })));

        // Assert
        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void DamageModal_ShouldRender_WhenVisible()
    {
        // Arrange & Act
        var cut = RenderComponent<DamageModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Amount, 5)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { }))
            .Add(p => p.OnApply, EventCallback.Factory.Create(this, () => { })));

        // Assert
        var modal = cut.Find(".modal.show");
        Assert.NotNull(modal);
    }

    [Fact]
    public void DamageModal_ShouldDisplayTitle()
    {
        // Arrange & Act
        var cut = RenderComponent<DamageModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Amount, 5)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { }))
            .Add(p => p.OnApply, EventCallback.Factory.Create(this, () => { })));

        // Assert
        Assert.Contains("Apply Damage", cut.Markup);
    }

    [Fact]
    public void DamageModal_ShouldDisplayAmountInput()
    {
        // Arrange & Act
        var cut = RenderComponent<DamageModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Amount, 10)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { }))
            .Add(p => p.OnApply, EventCallback.Factory.Create(this, () => { })));

        // Assert
        var input = cut.Find("input[type='number']");
        Assert.NotNull(input);
    }

    [Fact]
    public void DamageModal_ShouldHaveQuickSelectButtons()
    {
        // Arrange & Act
        var cut = RenderComponent<DamageModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Amount, 5)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { }))
            .Add(p => p.OnApply, EventCallback.Factory.Create(this, () => { })));

        // Assert
        var buttons = cut.FindAll("button");
        var quickSelectButtons = buttons.Where(b => 
            b.TextContent == "5" || 
            b.TextContent == "10" || 
            b.TextContent == "15" || 
            b.TextContent == "20");
        
        Assert.Equal(4, quickSelectButtons.Count());
    }

    [Fact]
    public void DamageModal_ShouldHaveCancelButton()
    {
        // Arrange & Act
        var cut = RenderComponent<DamageModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Amount, 5)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { }))
            .Add(p => p.OnApply, EventCallback.Factory.Create(this, () => { })));

        // Assert
        var cancelButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Cancel"));
        Assert.NotNull(cancelButton);
    }

    [Fact]
    public void DamageModal_ShouldHaveApplyButton()
    {
        // Arrange & Act
        var cut = RenderComponent<DamageModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Amount, 5)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { }))
            .Add(p => p.OnApply, EventCallback.Factory.Create(this, () => { })));

        // Assert
        var applyButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Apply Damage"));
        Assert.NotNull(applyButton);
        Assert.Contains("btn-danger", applyButton.ClassName);
    }

    [Fact]
    public void DamageModal_ShouldCallOnClose_WhenCancelClicked()
    {
        // Arrange
        var closeCalled = false;
        
        var cut = RenderComponent<DamageModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Amount, 5)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => closeCalled = true))
            .Add(p => p.OnApply, EventCallback.Factory.Create(this, () => { })));

        // Act
        var cancelButton = cut.FindAll("button").First(b => b.TextContent.Contains("Cancel"));
        cancelButton.Click();

        // Assert
        Assert.True(closeCalled);
    }

    [Fact]
    public void DamageModal_ShouldCallOnApply_WhenApplyClicked()
    {
        // Arrange
        var applyCalled = false;
        
        var cut = RenderComponent<DamageModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Amount, 5)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { }))
            .Add(p => p.OnApply, EventCallback.Factory.Create(this, () => applyCalled = true)));

        // Act
        var applyButton = cut.FindAll("button").First(b => b.TextContent.Contains("Apply Damage"));
        applyButton.Click();

        // Assert
        Assert.True(applyCalled);
    }

    [Fact]
    public void DamageModal_ShouldNotCallOnApply_WhenAmountIsZero()
    {
        // Arrange
        var applyCalled = false;
        
        var cut = RenderComponent<DamageModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Amount, 0)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { }))
            .Add(p => p.OnApply, EventCallback.Factory.Create(this, () => applyCalled = true)));

        // Act
        var applyButton = cut.FindAll("button").First(b => b.TextContent.Contains("Apply Damage"));
        applyButton.Click();

        // Assert
        Assert.False(applyCalled);
    }

    [Fact]
    public void DamageModal_ShouldHaveCloseButtonInHeader()
    {
        // Arrange & Act
        var cut = RenderComponent<DamageModal>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Amount, 5)
            .Add(p => p.OnClose, EventCallback.Factory.Create(this, () => { }))
            .Add(p => p.OnApply, EventCallback.Factory.Create(this, () => { })));

        // Assert
        var closeButton = cut.Find(".btn-close");
        Assert.NotNull(closeButton);
    }
}
