using Bunit;
using CombatTracker.WebAssembly.Components.Shared;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Components.Shared;

/// <summary>
/// Tests for the LoadingSpinner component
/// </summary>
public class LoadingSpinnerTests : TestContext
{
    [Fact]
    public void LoadingSpinner_ShouldNotRender_WhenIsVisibleIsFalse()
    {
        // Arrange & Act
        var cut = RenderComponent<LoadingSpinner>(parameters => parameters
            .Add(p => p.IsVisible, false));

        // Assert
        Assert.Empty(cut.Markup);
    }

    [Fact]
    public void LoadingSpinner_ShouldRender_WhenIsVisibleIsTrue()
    {
        // Arrange & Act
        var cut = RenderComponent<LoadingSpinner>(parameters => parameters
            .Add(p => p.IsVisible, true));

        // Assert
        var overlay = cut.Find(".loading-overlay");
        Assert.NotNull(overlay);
        
        var spinner = cut.Find(".loading-spinner");
        Assert.NotNull(spinner);
    }

    [Fact]
    public void LoadingSpinner_ShouldDisplayMessage_WhenMessageIsProvided()
    {
        // Arrange
        var message = "Processing data...";

        // Act
        var cut = RenderComponent<LoadingSpinner>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, message));

        // Assert
        Assert.Contains(message, cut.Markup);
    }

    [Fact]
    public void LoadingSpinner_ShouldNotDisplayMessage_WhenMessageIsEmpty()
    {
        // Arrange & Act
        var cut = RenderComponent<LoadingSpinner>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, ""));

        // Assert
        var content = cut.Find(".loading-content");
        var paragraphs = content.QuerySelectorAll("p");
        Assert.Empty(paragraphs);
    }

    [Fact]
    public void LoadingSpinner_ShouldApplyCssClass_WhenProvided()
    {
        // Arrange
        var cssClass = "custom-class";

        // Act
        var cut = RenderComponent<LoadingSpinner>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.CssClass, cssClass));

        // Assert
        var overlay = cut.Find(".loading-overlay");
        Assert.Contains(cssClass, overlay.ClassName);
    }
}
