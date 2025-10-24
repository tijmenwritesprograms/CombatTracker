using CombatTracker.Web.Models;
using CombatTracker.Web.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CombatTracker.Web.Tests.Services;

/// <summary>
/// Tests for the StorageStateService
/// </summary>
public class StorageStateServiceTests
{
    private readonly Mock<ILocalStorageService> _mockLocalStorage;
    private readonly StorageStateService _service;

    public StorageStateServiceTests()
    {
        _mockLocalStorage = new Mock<ILocalStorageService>();
        var storageLogger = new Mock<ILogger<StorageStateService>>();
        _service = new StorageStateService(_mockLocalStorage.Object, storageLogger.Object);
    }

    [Fact]
    public async Task SavePartiesAsync_ShouldCallLocalStorage()
    {
        // Arrange
        var parties = new List<Party>
        {
            new Party { Id = 1, Name = "Test Party" }
        };
        _mockLocalStorage.Setup(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<PartyStorageData>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.SavePartiesAsync(parties, 2, 1);

        // Assert
        Assert.True(result);
        _mockLocalStorage.Verify(x => x.SetItemAsync("combattracker_parties", It.IsAny<PartyStorageData>()), Times.Once);
    }

    [Fact]
    public async Task LoadPartiesAsync_ShouldReturnData()
    {
        // Arrange
        var expectedData = new PartyStorageData
        {
            Parties = new List<Party> { new Party { Id = 1, Name = "Test" } },
            NextPartyId = 2,
            NextCharacterId = 1
        };
        _mockLocalStorage.Setup(x => x.GetItemAsync<PartyStorageData>(It.IsAny<string>()))
            .ReturnsAsync(expectedData);

        // Act
        var result = await _service.LoadPartiesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Parties);
        Assert.Equal("Test", result.Parties[0].Name);
    }

    [Fact]
    public async Task SaveCombatStateAsync_WithNullData_ShouldRemoveFromStorage()
    {
        // Arrange
        _mockLocalStorage.Setup(x => x.RemoveItemAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.SaveCombatStateAsync(null);

        // Assert
        Assert.True(result);
        _mockLocalStorage.Verify(x => x.RemoveItemAsync("combattracker_combat_state"), Times.Once);
    }

    [Fact]
    public async Task SaveCombatStateAsync_WithData_ShouldSaveToStorage()
    {
        // Arrange
        var combatData = new CombatStorageData
        {
            ActiveCombat = new Combat { Id = 1, Round = 1 }
        };
        _mockLocalStorage.Setup(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<CombatStorageData>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.SaveCombatStateAsync(combatData);

        // Assert
        Assert.True(result);
        _mockLocalStorage.Verify(x => x.SetItemAsync("combattracker_combat_state", It.IsAny<CombatStorageData>()), Times.Once);
    }

    [Fact]
    public async Task ExportAllDataAsync_ShouldReturnJsonString()
    {
        // Arrange
        var parties = new List<Party> { new Party { Id = 1, Name = "Test Party" } };

        // Act
        var result = await _service.ExportAllDataAsync(parties, 2, 1, null);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Test Party", result);
        Assert.Contains("\"Version\":", result);
    }

    [Fact]
    public async Task ImportDataAsync_ShouldDeserializeData()
    {
        // Arrange
        var json = @"{
            ""PartyData"": {
                ""Parties"": [{""Id"": 1, ""Name"": ""Imported Party"", ""Characters"": []}],
                ""NextPartyId"": 2,
                ""NextCharacterId"": 1,
                ""Version"": 1,
                ""LastSaved"": ""2024-01-01T00:00:00Z""
            },
            ""CombatData"": null,
            ""ExportedAt"": ""2024-01-01T00:00:00Z"",
            ""Version"": 1
        }";

        // Act
        var (partyData, combatData) = await _service.ImportDataAsync(json);

        // Assert
        Assert.NotNull(partyData);
        Assert.Single(partyData.Parties);
        Assert.Equal("Imported Party", partyData.Parties[0].Name);
        Assert.Null(combatData);
    }

    [Fact]
    public async Task ClearAllDataAsync_ShouldRemoveAllKeys()
    {
        // Arrange
        _mockLocalStorage.Setup(x => x.RemoveItemAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.ClearAllDataAsync();

        // Assert
        Assert.True(result);
        _mockLocalStorage.Verify(x => x.RemoveItemAsync(It.IsAny<string>()), Times.AtLeast(2));
    }

    [Fact]
    public async Task OnStorageOperation_ShouldTriggerEvent()
    {
        // Arrange
        string? eventMessage = null;
        bool? eventSuccess = null;
        _service.OnStorageOperation += (message, success) =>
        {
            eventMessage = message;
            eventSuccess = success;
        };

        _mockLocalStorage.Setup(x => x.SetItemAsync(It.IsAny<string>(), It.IsAny<PartyStorageData>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.SavePartiesAsync(new List<Party>(), 1, 1);

        // Assert
        Assert.NotNull(eventMessage);
        Assert.True(eventSuccess);
    }
}
