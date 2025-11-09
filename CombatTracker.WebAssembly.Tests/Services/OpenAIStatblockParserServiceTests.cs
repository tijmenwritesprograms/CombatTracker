using System.Net;
using System.Text.Json;
using CombatTracker.WebAssembly.Models;
using CombatTracker.WebAssembly.Services;
using Moq;
using Moq.Protected;
using Xunit;

namespace CombatTracker.WebAssembly.Tests.Services;

/// <summary>
/// Tests for the OpenAIStatblockParserService
/// </summary>
public class OpenAIStatblockParserServiceTests
{
    private readonly Mock<IApiKeyService> _mockApiKeyService;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly OpenAIStatblockParserService _service;

    public OpenAIStatblockParserServiceTests()
    {
        _mockApiKeyService = new Mock<IApiKeyService>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new OpenAIStatblockParserService(_httpClient, _mockApiKeyService.Object);
    }

    [Fact]
    public async Task IsConfiguredAsync_ShouldReturnTrueWhenApiKeyExists()
    {
        // Arrange
        _mockApiKeyService.Setup(x => x.HasOpenAIApiKeyAsync())
            .ReturnsAsync(true);

        // Act
        var result = await _service.IsConfiguredAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsConfiguredAsync_ShouldReturnFalseWhenApiKeyDoesNotExist()
    {
        // Arrange
        _mockApiKeyService.Setup(x => x.HasOpenAIApiKeyAsync())
            .ReturnsAsync(false);

        // Act
        var result = await _service.IsConfiguredAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ParseStatblockAsync_ShouldThrowWhenApiKeyNotConfigured()
    {
        // Arrange
        _mockApiKeyService.Setup(x => x.GetOpenAIApiKeyAsync())
            .ReturnsAsync((string?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ParseStatblockAsync("test statblock"));
    }

    [Fact]
    public async Task ParseStatblockAsync_ShouldReturnNullForEmptyInput()
    {
        // Act
        var result = await _service.ParseStatblockAsync("");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ParseStatblockAsync_ShouldReturnNullForWhitespaceInput()
    {
        // Act
        var result = await _service.ParseStatblockAsync("   ");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ParseStatblockAsync_ShouldParseValidStatblock()
    {
        // Arrange
        var apiKey = "sk-test-key";
        _mockApiKeyService.Setup(x => x.GetOpenAIApiKeyAsync())
            .ReturnsAsync(apiKey);

        var expectedMonster = new Monster
        {
            Name = "Orc",
            Size = "Medium",
            Type = "Humanoid",
            Subtype = "Orc",
            Alignment = "Chaotic Evil",
            AC = 13,
            Hp = 15,
            InitiativeModifier = 1
        };

        var openAIResponse = new
        {
            id = "test-id",
            @object = "chat.completion",
            created = 1234567890,
            model = "gpt-4o-mini",
            choices = new[]
            {
                new
                {
                    index = 0,
                    message = new
                    {
                        role = "assistant",
                        content = JsonSerializer.Serialize(expectedMonster)
                    },
                    finish_reason = "stop"
                }
            }
        };

        var responseContent = new StringContent(JsonSerializer.Serialize(openAIResponse));
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = responseContent
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _service.ParseStatblockAsync("test statblock");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Orc", result.Name);
        Assert.Equal("Medium", result.Size);
        Assert.Equal("Humanoid", result.Type);
        Assert.Equal(13, result.AC);
        Assert.Equal(15, result.Hp);
    }

    [Fact]
    public async Task ParseStatblockAsync_ShouldThrowOnHttpError()
    {
        // Arrange
        var apiKey = "sk-test-key";
        _mockApiKeyService.Setup(x => x.GetOpenAIApiKeyAsync())
            .ReturnsAsync(apiKey);

        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("Invalid API key")
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ParseStatblockAsync("test statblock"));
    }

    [Fact]
    public async Task ParseStatblockAsync_ShouldThrowOnEmptyResponse()
    {
        // Arrange
        var apiKey = "sk-test-key";
        _mockApiKeyService.Setup(x => x.GetOpenAIApiKeyAsync())
            .ReturnsAsync(apiKey);

        var openAIResponse = new
        {
            id = "test-id",
            @object = "chat.completion",
            created = 1234567890,
            model = "gpt-4o-mini",
            choices = Array.Empty<object>()
        };

        var responseContent = new StringContent(JsonSerializer.Serialize(openAIResponse));
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = responseContent
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ParseStatblockAsync("test statblock"));
    }

    [Fact]
    public async Task ParseStatblockAsync_ShouldRetryOnTransientFailure()
    {
        // Arrange
        var apiKey = "sk-test-key";
        _mockApiKeyService.Setup(x => x.GetOpenAIApiKeyAsync())
            .ReturnsAsync(apiKey);

        var expectedMonster = new Monster
        {
            Name = "Goblin",
            Type = "Humanoid",
            AC = 15,
            Hp = 7
        };

        var openAIResponse = new
        {
            id = "test-id",
            @object = "chat.completion",
            created = 1234567890,
            model = "gpt-4o-mini",
            choices = new[]
            {
                new
                {
                    index = 0,
                    message = new
                    {
                        role = "assistant",
                        content = JsonSerializer.Serialize(expectedMonster)
                    },
                    finish_reason = "stop"
                }
            }
        };

        var callCount = 0;
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    // First call fails with network error
                    throw new HttpRequestException("Network error");
                }
                // Second call succeeds
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(openAIResponse))
                };
            });

        // Act
        var result = await _service.ParseStatblockAsync("test statblock");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Goblin", result.Name);
        Assert.Equal(2, callCount); // Should have retried once
    }

    [Fact]
    public async Task ParseStatblockAsync_ShouldParseComplexStatblockWithTraitsAndActions()
    {
        // Arrange
        var apiKey = "sk-test-key";
        _mockApiKeyService.Setup(x => x.GetOpenAIApiKeyAsync())
            .ReturnsAsync(apiKey);

        var expectedMonster = new Monster
        {
            Name = "Orc Warrior",
            Size = "Medium",
            Type = "Humanoid",
            Subtype = "Orc",
            AC = 13,
            Hp = 15,
            InitiativeModifier = 1,
            Traits = new List<MonsterTrait>
            {
                new MonsterTrait
                {
                    Name = "Aggressive",
                    Description = "As a bonus action, the orc can move up to its speed toward a hostile creature."
                }
            },
            Actions = new List<MonsterAction>
            {
                new MonsterAction
                {
                    Name = "Greataxe",
                    Description = "Melee Weapon Attack: +5 to hit, reach 5 ft., one target.",
                    AttackType = "Melee Weapon Attack",
                    AttackBonus = 5,
                    Reach = 5,
                    DamageFormula = "1d12 + 3",
                    DamageType = "slashing"
                }
            }
        };

        var openAIResponse = new
        {
            id = "test-id",
            @object = "chat.completion",
            created = 1234567890,
            model = "gpt-4o-mini",
            choices = new[]
            {
                new
                {
                    index = 0,
                    message = new
                    {
                        role = "assistant",
                        content = JsonSerializer.Serialize(expectedMonster)
                    },
                    finish_reason = "stop"
                }
            }
        };

        var responseContent = new StringContent(JsonSerializer.Serialize(openAIResponse));
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = responseContent
        };

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await _service.ParseStatblockAsync("test statblock with traits and actions");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Orc Warrior", result.Name);
        Assert.Single(result.Traits);
        Assert.Equal("Aggressive", result.Traits[0].Name);
        Assert.Single(result.Actions);
        Assert.Equal("Greataxe", result.Actions[0].Name);
        Assert.Equal(5, result.Actions[0].AttackBonus);
    }
}
