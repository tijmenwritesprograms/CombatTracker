using CombatTracker.Web.Models;
using System.Text.Json;

namespace CombatTracker.Web.Services;

/// <summary>
/// Service for managing persistent storage of party and combat state.
/// </summary>
public class StorageStateService
{
    private readonly LocalStorageService _localStorage;
    private readonly ILogger<StorageStateService> _logger;

    private const string PartiesKey = "combattracker_parties";
    private const string CombatStateKey = "combattracker_combat_state";
    private const string StorageVersionKey = "combattracker_storage_version";
    private const int CurrentStorageVersion = 1;

    public StorageStateService(LocalStorageService localStorage, ILogger<StorageStateService> logger)
    {
        _localStorage = localStorage;
        _logger = logger;
    }

    /// <summary>
    /// Event raised when storage operation completes (success or failure).
    /// </summary>
    public event Action<string, bool>? OnStorageOperation;

    #region Party Data Persistence

    /// <summary>
    /// Saves party data to localStorage.
    /// </summary>
    public async Task<bool> SavePartiesAsync(IEnumerable<Party> parties, int nextPartyId, int nextCharacterId)
    {
        try
        {
            var data = new PartyStorageData
            {
                Parties = parties.ToList(),
                NextPartyId = nextPartyId,
                NextCharacterId = nextCharacterId,
                Version = CurrentStorageVersion,
                LastSaved = DateTime.UtcNow
            };

            var success = await _localStorage.SetItemAsync(PartiesKey, data);
            OnStorageOperation?.Invoke("Party data saved", success);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving party data");
            OnStorageOperation?.Invoke("Error saving party data", false);
            return false;
        }
    }

    /// <summary>
    /// Loads party data from localStorage.
    /// </summary>
    public async Task<PartyStorageData?> LoadPartiesAsync()
    {
        try
        {
            var data = await _localStorage.GetItemAsync<PartyStorageData>(PartiesKey);
            if (data != null)
            {
                _logger.LogInformation("Loaded {Count} parties from storage", data.Parties.Count);
                OnStorageOperation?.Invoke("Party data loaded", true);
            }
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading party data");
            OnStorageOperation?.Invoke("Error loading party data", false);
            return null;
        }
    }

    #endregion

    #region Combat State Persistence

    /// <summary>
    /// Saves active combat state to localStorage.
    /// </summary>
    public async Task<bool> SaveCombatStateAsync(CombatStorageData? combatData)
    {
        try
        {
            if (combatData == null)
            {
                // No active combat, remove from storage
                return await _localStorage.RemoveItemAsync(CombatStateKey);
            }

            combatData.Version = CurrentStorageVersion;
            combatData.LastSaved = DateTime.UtcNow;

            var success = await _localStorage.SetItemAsync(CombatStateKey, combatData);
            OnStorageOperation?.Invoke("Combat state saved", success);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving combat state");
            OnStorageOperation?.Invoke("Error saving combat state", false);
            return false;
        }
    }

    /// <summary>
    /// Loads active combat state from localStorage.
    /// </summary>
    public async Task<CombatStorageData?> LoadCombatStateAsync()
    {
        try
        {
            var data = await _localStorage.GetItemAsync<CombatStorageData>(CombatStateKey);
            if (data != null)
            {
                _logger.LogInformation("Loaded combat state from storage (Round {Round})", data.ActiveCombat?.Round ?? 0);
                OnStorageOperation?.Invoke("Combat state loaded", true);
            }
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading combat state");
            OnStorageOperation?.Invoke("Error loading combat state", false);
            return null;
        }
    }

    #endregion

    #region Import/Export

    /// <summary>
    /// Exports all data as JSON string.
    /// </summary>
    public async Task<string?> ExportAllDataAsync(IEnumerable<Party> parties, int nextPartyId, int nextCharacterId, CombatStorageData? combatData)
    {
        try
        {
            var exportData = new ExportData
            {
                PartyData = new PartyStorageData
                {
                    Parties = parties.ToList(),
                    NextPartyId = nextPartyId,
                    NextCharacterId = nextCharacterId,
                    Version = CurrentStorageVersion,
                    LastSaved = DateTime.UtcNow
                },
                CombatData = combatData,
                ExportedAt = DateTime.UtcNow,
                Version = CurrentStorageVersion
            };

            return JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data");
            return null;
        }
    }

    /// <summary>
    /// Imports data from JSON string.
    /// </summary>
    public async Task<(PartyStorageData?, CombatStorageData?)> ImportDataAsync(string json)
    {
        try
        {
            var exportData = JsonSerializer.Deserialize<ExportData>(json);
            if (exportData == null)
            {
                _logger.LogWarning("Failed to deserialize import data");
                return (null, null);
            }

            return (exportData.PartyData, exportData.CombatData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing data");
            return (null, null);
        }
    }

    #endregion

    #region Clear Data

    /// <summary>
    /// Clears all Combat Tracker data from localStorage.
    /// </summary>
    public async Task<bool> ClearAllDataAsync()
    {
        try
        {
            var partiesRemoved = await _localStorage.RemoveItemAsync(PartiesKey);
            var combatRemoved = await _localStorage.RemoveItemAsync(CombatStateKey);
            var versionRemoved = await _localStorage.RemoveItemAsync(StorageVersionKey);

            var success = partiesRemoved && combatRemoved;
            OnStorageOperation?.Invoke("All data cleared", success);
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing all data");
            OnStorageOperation?.Invoke("Error clearing all data", false);
            return false;
        }
    }

    #endregion
}

#region Storage Data Models

/// <summary>
/// Data structure for storing party information.
/// </summary>
public class PartyStorageData
{
    public List<Party> Parties { get; set; } = new();
    public int NextPartyId { get; set; }
    public int NextCharacterId { get; set; }
    public int Version { get; set; }
    public DateTime LastSaved { get; set; }
}

/// <summary>
/// Data structure for storing combat state.
/// </summary>
public class CombatStorageData
{
    public Combat? ActiveCombat { get; set; }
    public List<CombatLogEntry> CombatLog { get; set; } = new();
    public Dictionary<string, CombatantSetupData> Combatants { get; set; } = new();
    public List<Monster> Monsters { get; set; } = new();
    public Party? SelectedParty { get; set; }
    public int NextMonsterId { get; set; }
    public Dictionary<string, int> CombatantKeyMapping { get; set; } = new();
    public int Version { get; set; }
    public DateTime LastSaved { get; set; }
}

/// <summary>
/// Data structure for import/export.
/// </summary>
public class ExportData
{
    public PartyStorageData? PartyData { get; set; }
    public CombatStorageData? CombatData { get; set; }
    public DateTime ExportedAt { get; set; }
    public int Version { get; set; }
}

#endregion
