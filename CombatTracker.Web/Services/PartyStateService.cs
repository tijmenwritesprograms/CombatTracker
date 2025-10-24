using CombatTracker.Web.Models;

namespace CombatTracker.Web.Services;

/// <summary>
/// In-memory state management service for parties and characters.
/// </summary>
public class PartyStateService
{
    private readonly List<Party> _parties = new();
    private int _nextPartyId = 1;
    private int _nextCharacterId = 1;

    /// <summary>
    /// Event raised when party data changes.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyStateService"/> class.
    /// Seeding is performed via SeedSampleParty method on demand.
    /// </summary>
    public PartyStateService()
    {
        // Intentionally empty. Use SeedSampleParty() to add test data on demand.
    }

    /// <summary>
    /// Seeds a sample party of 4 adventurers for testing purposes.
    /// </summary>
    public void SeedSampleParty()
    {
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
    public Party CreateParty(string name)
    {
        var party = new Party
        {
            Id = _nextPartyId++,
            Name = name
        };
        _parties.Add(party);
        NotifyStateChanged();
        return party;
    }

    /// <summary>
    /// Updates an existing party.
    /// </summary>
    public void UpdateParty(Party party)
    {
        var existingParty = GetPartyById(party.Id);
        if (existingParty != null)
        {
            existingParty.Name = party.Name;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Deletes a party by ID.
    /// </summary>
    public void DeleteParty(int id)
    {
        var party = GetPartyById(id);
        if (party != null)
        {
            _parties.Remove(party);
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Adds a character to a party.
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
        NotifyStateChanged();
        return character;
    }

    /// <summary>
    /// Updates a character in a party.
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
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Deletes a character from a party.
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
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
