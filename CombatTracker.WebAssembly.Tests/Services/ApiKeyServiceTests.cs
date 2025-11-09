using CombatTracker.WebAssembly.Services;
using Moq;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Services;

/// <summary>
/// Tests for the ApiKeyService
/// </summary>
public class ApiKeyServiceTests
{
    private readonly Mock<ILocalStorageService> _mockLocalStorage;
    private readonly ApiKeyService _service;

    public ApiKeyServiceTests()
    {
        _mockLocalStorage = new Mock<ILocalStorageService>();
        _service = new ApiKeyService(_mockLocalStorage.Object);
    }

    [Fact]
    public async Task InitializeAsync_ShouldLoadApiKeyFromStorage()
    {
        // Arrange
        var expectedKey = "sk-test-key";
        _mockLocalStorage.Setup(x => x.GetItemAsync<string>("openai_api_key"))
            .ReturnsAsync(expectedKey);

        // Act
        await _service.InitializeAsync();
        var result = await _service.GetOpenAIApiKeyAsync();

        // Assert
        Assert.Equal(expectedKey, result);
    }

    [Fact]
    public async Task GetOpenAIApiKeyAsync_ShouldReturnNullWhenNotSet()
    {
        // Arrange
        _mockLocalStorage.Setup(x => x.GetItemAsync<string>("openai_api_key"))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _service.GetOpenAIApiKeyAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetOpenAIApiKeyAsync_ShouldSaveToLocalStorage()
    {
        // Arrange
        var apiKey = "sk-test-key-123";
        _mockLocalStorage.Setup(x => x.SetItemAsync("openai_api_key", apiKey))
            .ReturnsAsync(true);

        // Act
        await _service.SetOpenAIApiKeyAsync(apiKey);

        // Assert
        _mockLocalStorage.Verify(x => x.SetItemAsync("openai_api_key", apiKey), Times.Once);
    }

    [Fact]
    public async Task SetOpenAIApiKeyAsync_ShouldRemoveWhenNull()
    {
        // Arrange
        _mockLocalStorage.Setup(x => x.RemoveItemAsync("openai_api_key"))
            .ReturnsAsync(true);

        // Act
        await _service.SetOpenAIApiKeyAsync(null);

        // Assert
        _mockLocalStorage.Verify(x => x.RemoveItemAsync("openai_api_key"), Times.Once);
        _mockLocalStorage.Verify(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SetOpenAIApiKeyAsync_ShouldRemoveWhenEmpty()
    {
        // Arrange
        _mockLocalStorage.Setup(x => x.RemoveItemAsync("openai_api_key"))
            .ReturnsAsync(true);

        // Act
        await _service.SetOpenAIApiKeyAsync("");

        // Assert
        _mockLocalStorage.Verify(x => x.RemoveItemAsync("openai_api_key"), Times.Once);
        _mockLocalStorage.Verify(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task SetOpenAIApiKeyAsync_ShouldRemoveWhenWhitespace()
    {
        // Arrange
        _mockLocalStorage.Setup(x => x.RemoveItemAsync("openai_api_key"))
            .ReturnsAsync(true);

        // Act
        await _service.SetOpenAIApiKeyAsync("   ");

        // Assert
        _mockLocalStorage.Verify(x => x.RemoveItemAsync("openai_api_key"), Times.Once);
        _mockLocalStorage.Verify(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task HasOpenAIApiKeyAsync_ShouldReturnTrueWhenKeyExists()
    {
        // Arrange
        _mockLocalStorage.Setup(x => x.GetItemAsync<string>("openai_api_key"))
            .ReturnsAsync("sk-test-key");

        // Act
        var result = await _service.HasOpenAIApiKeyAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasOpenAIApiKeyAsync_ShouldReturnFalseWhenKeyIsNull()
    {
        // Arrange
        _mockLocalStorage.Setup(x => x.GetItemAsync<string>("openai_api_key"))
            .ReturnsAsync((string?)null);

        // Act
        var result = await _service.HasOpenAIApiKeyAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasOpenAIApiKeyAsync_ShouldReturnFalseWhenKeyIsEmpty()
    {
        // Arrange
        _mockLocalStorage.Setup(x => x.GetItemAsync<string>("openai_api_key"))
            .ReturnsAsync("");

        // Act
        var result = await _service.HasOpenAIApiKeyAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasOpenAIApiKeyAsync_ShouldReturnFalseWhenKeyIsWhitespace()
    {
        // Arrange
        _mockLocalStorage.Setup(x => x.GetItemAsync<string>("openai_api_key"))
            .ReturnsAsync("   ");

        // Act
        var result = await _service.HasOpenAIApiKeyAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SetOpenAIApiKeyAsync_ShouldInvokeOnChangeEvent()
    {
        // Arrange
        var eventRaised = false;
        _service.OnChange += () => eventRaised = true;
        _mockLocalStorage.Setup(x => x.SetItemAsync("openai_api_key", It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        await _service.SetOpenAIApiKeyAsync("sk-test-key");

        // Assert
        Assert.True(eventRaised);
    }

    [Fact]
    public async Task SetOpenAIApiKeyAsync_ShouldCacheKey()
    {
        // Arrange
        var apiKey = "sk-test-key";
        _mockLocalStorage.Setup(x => x.SetItemAsync("openai_api_key", apiKey))
            .ReturnsAsync(true);

        // Act
        await _service.SetOpenAIApiKeyAsync(apiKey);
        var result = await _service.GetOpenAIApiKeyAsync();

        // Assert
        Assert.Equal(apiKey, result);
        // Verify GetItemAsync was not called again (cached)
        _mockLocalStorage.Verify(x => x.GetItemAsync<string>("openai_api_key"), Times.Never);
    }
}
