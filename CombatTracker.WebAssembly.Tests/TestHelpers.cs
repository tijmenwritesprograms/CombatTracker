using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using CombatTracker.WebAssembly.Services;

namespace CombatTracker.WebAssembly.Tests;

/// <summary>
/// Helper class for creating test dependencies
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// Creates a mock logger for the specified type
    /// </summary>
    public static ILogger<T> CreateMockLogger<T>()
    {
        return new Mock<ILogger<T>>().Object;
    }

    /// <summary>
    /// Creates a mock KeyboardShortcutService for testing
    /// </summary>
    public static KeyboardShortcutService CreateMockKeyboardShortcutService()
    {
        var mockJsRuntime = new Mock<IJSRuntime>();
        
        // Setup mock to return null for import calls (initialization will fail gracefully)
        mockJsRuntime
            .Setup(x => x.InvokeAsync<IJSObjectReference>(
                It.IsAny<string>(), 
                It.IsAny<object[]>()))
            .ThrowsAsync(new JSException("Mock JS module not available"));
        
        return new KeyboardShortcutService(mockJsRuntime.Object);
    }
}
