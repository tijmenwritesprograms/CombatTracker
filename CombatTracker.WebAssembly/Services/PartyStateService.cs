
using CombatTracker.WebAssembly.Models;
using Microsoft.Extensions.Logging;

namespace CombatTracker.WebAssembly.Services;

/// <summary>
/// In-memory state management service for parties and characters with local storage persistence.
/// </summary>
public class PartyStateService
{
    private readonly List<Party> _parties = new();
    private int _nextPartyId = 1;
    private int _nextCharacterId = 1;
    private readonly StorageStateService? _storageService;
    private readonly ILogger<PartyStateService> _logger;
    private bool _isInitialized = false;

    /// <summary>
    /// Event raised when party data changes.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyStateService"/> class.
    /// Seeding is performed via SeedSampleParty method on demand.
    /// </summary>
    public PartyStateService(ILogger<PartyStateService> logger, StorageStateService? storageService = null)
    {
        _logger = logger;
        _storageService = storageService;
    }

    /// <summary>
    /// Initializes the service and loads data from storage if available.
    /// Must be called from a component after render.
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        if (_storageService != null)
        {
            await LoadFromStorageAsync();
        }
    }

    /// <summary>
    /// Seeds a sample party of 4 adventurers for testing purposes.
    /// </summary>
    public void SeedSampleParty()
    {
#if DEBUG
        // Avoid duplicating the same seeded party multiple times by checking existing names.
        if (_parties.Any(p => p.Name == "The Silver Blades"))
        {
            return;
        }

        var seededParty = CreateParty("The Silver Blades");

        AddCharacter(seededParty.Id, new Character
        {
            Name = "Aelar",
            Class = "Fighter",
            Level = 5,
            HpMax = 44,
            HpCurrent = 44,
            AC = 18,
            InitiativeModifier = 2,
            Notes = "Human fighter, frontline tank"
        });

        AddCharacter(seededParty.Id, new Character
        {
            Name = "Lyra",
            Class = "Wizard",
            Level = 5,
            HpMax = 24,
            HpCurrent = 24,
            AC = 12,
            InitiativeModifier = 1,
            Notes = "Elven wizard, ranged caster"
        });

        AddCharacter(seededParty.Id, new Character
        {
            Name = "Thokk",
            Class = "Rogue",
            Level = 5,
            HpMax = 34,
            HpCurrent = 34,
            AC = 15,
            InitiativeModifier = 4,
            Notes = "Halfling rogue, scout and skirmisher"
        });

        AddCharacter(seededParty.Id, new Character
        {
            Name = "Miri",
            Class = "Cleric",
            Level = 5,
            HpMax = 40,
            HpCurrent = 40,
            AC = 17,
            InitiativeModifier = 0,
            Notes = "Dwarf cleric, healer and support"
        });

        // Final notification (CreateParty and AddCharacter already trigger notifications, but ensure observers are updated)
        NotifyStateChanged();
#endif
    }

    /// <summary>
    /// Gets all parties.
    /// </summary>
    public IReadOnlyList<Party> GetAllParties() => _parties.AsReadOnly();

    /// <summary>
    /// Gets a party by ID.
    /// </summary>
    public Party? GetPartyById(int id) => _parties.FirstOrDefault(p => p.Id == id);

    /// <summary>
    /// Creates a new party.
    /// </summary>
    public async Task<Party> CreatePartyAsync(string name)
    {
        var party = new Party
        {
            Id = _nextPartyId++,
            Name = name
        };
        _parties.Add(party);
        await SaveToStorageAsync();
        NotifyStateChanged();
        return party;
    }

    /// <summary>
    /// Creates a new party (synchronous version for backward compatibility).
    /// </summary>
    public Party CreateParty(string name)
    {
        var party = new Party
        {
            Id = _nextPartyId++,
            Name = name
        };
        _parties.Add(party);
        _ = SaveToStorageAsync(); // Fire and forget
        NotifyStateChanged();
        return party;
    }

    /// <summary>
    /// Updates an existing party.
    /// </summary>
    public async Task UpdatePartyAsync(Party party)
    {
        var existingParty = GetPartyById(party.Id);
        if (existingParty != null)
        {
            existingParty.Name = party.Name;
            await SaveToStorageAsync();
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Updates an existing party (synchronous version for backward compatibility).
    /// </summary>
    public void UpdateParty(Party party)
    {
        var existingParty = GetPartyById(party.Id);
        if (existingParty != null)
        {
            existingParty.Name = party.Name;
            _ = SaveToStorageAsync(); // Fire and forget
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Deletes a party by ID.
    /// </summary>
    public async Task DeletePartyAsync(int id)
    {
        var party = GetPartyById(id);
        if (party != null)
        {
            _parties.Remove(party);
            await SaveToStorageAsync();
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Deletes a party by ID (synchronous version for backward compatibility).
    /// </summary>
    public void DeleteParty(int id)
    {
        var party = GetPartyById(id);
        if (party != null)
        {
            _parties.Remove(party);
            _ = SaveToStorageAsync(); // Fire and forget
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Adds a character to a party.
    /// </summary>
    public async Task<Character> AddCharacterAsync(int partyId, Character character)
    {
        var party = GetPartyById(partyId);
        if (party == null)
        {
            throw new ArgumentException($"Party with ID {partyId} not found.", nameof(partyId));
        }

        character.Id = _nextCharacterId++;
        party.Characters.Add(character);
        await SaveToStorageAsync();
        NotifyStateChanged();
        return character;
    }

    /// <summary>
    /// Adds a character to a party (synchronous version for backward compatibility).
    /// </summary>
    public Character AddCharacter(int partyId, Character character)
    {
        var party = GetPartyById(partyId);
        if (party == null)
        {
            throw new ArgumentException($"Party with ID {partyId} not found.", nameof(partyId));
        }

        character.Id = _nextCharacterId++;
        party.Characters.Add(character);
        _ = SaveToStorageAsync(); // Fire and forget
        NotifyStateChanged();
        return character;
    }

    /// <summary>
    /// Updates a character in a party.
    /// </summary>
    public async Task UpdateCharacterAsync(int partyId, Character character)
    {
        var party = GetPartyById(partyId);
        if (party == null)
        {
            throw new ArgumentException($"Party with ID {partyId} not found.", nameof(partyId));
        }

        var existingCharacter = party.Characters.FirstOrDefault(c => c.Id == character.Id);
        if (existingCharacter != null)
        {
            existingCharacter.Name = character.Name;
            existingCharacter.Class = character.Class;
            existingCharacter.Level = character.Level;
            existingCharacter.HpCurrent = character.HpCurrent;
            existingCharacter.HpMax = character.HpMax;
            existingCharacter.AC = character.AC;
            existingCharacter.InitiativeModifier = character.InitiativeModifier;
            existingCharacter.Notes = character.Notes;
            await SaveToStorageAsync();
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Updates a character in a party (synchronous version for backward compatibility).
    /// </summary>
    public void UpdateCharacter(int partyId, Character character)
    {
        var party = GetPartyById(partyId);
        if (party == null)
        {
            throw new ArgumentException($"Party with ID {partyId} not found.", nameof(partyId));
        }

        var existingCharacter = party.Characters.FirstOrDefault(c => c.Id == character.Id);
        if (existingCharacter != null)
        {
            existingCharacter.Name = character.Name;
            existingCharacter.Class = character.Class;
            existingCharacter.Level = character.Level;
            existingCharacter.HpCurrent = character.HpCurrent;
            existingCharacter.HpMax = character.HpMax;
            existingCharacter.AC = character.AC;
            existingCharacter.InitiativeModifier = character.InitiativeModifier;
            existingCharacter.Notes = character.Notes;
            _ = SaveToStorageAsync(); // Fire and forget
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Deletes a character from a party.
    /// </summary>
    public async Task DeleteCharacterAsync(int partyId, int characterId)
    {
        var party = GetPartyById(partyId);
        if (party == null)
        {
            throw new ArgumentException($"Party with ID {partyId} not found.", nameof(partyId));
        }

        var character = party.Characters.FirstOrDefault(c => c.Id == characterId);
        if (character != null)
        {
            party.Characters.Remove(character);
            await SaveToStorageAsync();
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Deletes a character from a party (synchronous version for backward compatibility).
    /// </summary>
    public void DeleteCharacter(int partyId, int characterId)
    {
        var party = GetPartyById(partyId);
        if (party == null)
        {
            throw new ArgumentException($"Party with ID {partyId} not found.", nameof(partyId));
        }

        var character = party.Characters.FirstOrDefault(c => c.Id == characterId);
        if (character != null)
        {
            party.Characters.Remove(character);
            _ = SaveToStorageAsync(); // Fire and forget
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Loads party data from storage.
    /// </summary>
    private async Task LoadFromStorageAsync()
    {
        if (_storageService == null)
        {
            return;
        }

        try
        {
            var data = await _storageService.LoadPartiesAsync();
            if (data != null && data.Parties.Count > 0)
            {
                _parties.Clear();
                _parties.AddRange(data.Parties);
                _nextPartyId = data.NextPartyId;
                _nextCharacterId = data.NextCharacterId;
                _logger.LogInformation("Loaded {Count} parties from storage", data.Parties.Count);
                NotifyStateChanged();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading parties from storage");
        }
    }

    /// <summary>
    /// Saves party data to storage.
    /// </summary>
    private async Task SaveToStorageAsync()
    {
        if (_storageService == null)
        {
            return;
        }

        try
        {
            await _storageService.SavePartiesAsync(_parties, _nextPartyId, _nextCharacterId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving parties to storage");
        }
    }

    /// <summary>
    /// Gets internal state for export/import.
    /// </summary>
    public (List<Party> Parties, int NextPartyId, int NextCharacterId) GetInternalState()
    {
        return (_parties.ToList(), _nextPartyId, _nextCharacterId);
    }

    /// <summary>
    /// Restores internal state from import.
    /// </summary>
    public async Task RestoreInternalStateAsync(List<Party> parties, int nextPartyId, int nextCharacterId)
    {
        _parties.Clear();
        _parties.AddRange(parties);
        _nextPartyId = nextPartyId;
        _nextCharacterId = nextCharacterId;
        await SaveToStorageAsync();
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
