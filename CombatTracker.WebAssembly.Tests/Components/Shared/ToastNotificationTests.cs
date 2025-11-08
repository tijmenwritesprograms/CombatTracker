using Bunit;
using CombatTracker.WebAssembly.Components.Shared;
using Xunit;
using static CombatTracker.WebAssembly.Components.Shared.ToastNotification;

namespace CombatTracker.WebAssembly.Tests.Components.Shared;

/// <summary>
/// Tests for the ToastNotification component
/// </summary>
public class ToastNotificationTests : TestContext
{
    [Fact]
    public void ToastNotification_ShouldNotRenderToast_WhenIsVisibleIsFalse()
    {
        // Arrange & Act
        var cut = RenderComponent<ToastNotification>(parameters => parameters
            .Add(p => p.IsVisible, false));

        // Assert - component should not render the toast container
        var toastContainers = cut.FindAll(".toast-container");
        Assert.Empty(toastContainers);
    }

    [Fact]
    public void ToastNotification_ShouldRender_WhenIsVisibleIsTrue()
    {
        // Arrange & Act
        var cut = RenderComponent<ToastNotification>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, "Test message"));

        // Assert
        var toast = cut.Find(".toast");
        Assert.NotNull(toast);
    }

    [Fact]
    public void ToastNotification_ShouldDisplayMessage()
    {
        // Arrange
        var message = "Operation completed successfully!";

        // Act
        var cut = RenderComponent<ToastNotification>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, message));

        // Assert
        Assert.Contains(message, cut.Markup);
    }

    [Fact]
    public void ToastNotification_ShouldDisplaySuccessIcon_WhenTypeIsSuccess()
    {
        // Arrange & Act
        var cut = RenderComponent<ToastNotification>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Type, ToastType.Success)
            .Add(p => p.Message, "Success"));

        // Assert
        var icon = cut.Find(".bi-check-circle-fill");
        Assert.NotNull(icon);
        Assert.Contains("Success", cut.Markup);
    }

    [Fact]
    public void ToastNotification_ShouldDisplayErrorIcon_WhenTypeIsError()
    {
        // Arrange & Act
        var cut = RenderComponent<ToastNotification>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Type, ToastType.Error)
            .Add(p => p.Message, "Error"));

        // Assert
        var icon = cut.Find(".bi-exclamation-circle-fill");
        Assert.NotNull(icon);
        Assert.Contains("Error", cut.Markup);
    }

    [Fact]
    public void ToastNotification_ShouldDisplayWarningIcon_WhenTypeIsWarning()
    {
        // Arrange & Act
        var cut = RenderComponent<ToastNotification>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Type, ToastType.Warning)
            .Add(p => p.Message, "Warning"));

        // Assert
        var icon = cut.Find(".bi-exclamation-triangle-fill");
        Assert.NotNull(icon);
        Assert.Contains("Warning", cut.Markup);
    }

    [Fact]
    public void ToastNotification_ShouldDisplayInfoIcon_WhenTypeIsInfo()
    {
        // Arrange & Act
        var cut = RenderComponent<ToastNotification>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Type, ToastType.Info)
            .Add(p => p.Message, "Info"));

        // Assert
        var icon = cut.Find(".bi-info-circle-fill");
        Assert.NotNull(icon);
        Assert.Contains("Info", cut.Markup);
    }

    [Fact]
    public void ToastNotification_ShouldCallOnDismiss_WhenCloseButtonClicked()
    {
        // Arrange
        var dismissed = false;
        var cut = RenderComponent<ToastNotification>(parameters => parameters
            .Add(p => p.IsVisible, true)
            .Add(p => p.Message, "Test")
            .Add(p => p.OnDismiss, () => { dismissed = true; return Task.CompletedTask; }));

        // Act
        var closeButton = cut.Find(".btn-close");
        closeButton.Click();

        // Assert
        Assert.True(dismissed);
    }
}
