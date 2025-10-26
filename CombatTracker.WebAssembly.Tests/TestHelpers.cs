using Microsoft.Extensions.Logging;
using Moq;

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
}
