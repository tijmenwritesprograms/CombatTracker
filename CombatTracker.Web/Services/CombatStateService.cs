using CombatTracker.Web.Models;

namespace CombatTracker.Web.Services;

/// <summary>
/// In-memory state management service for combat encounters.
/// </summary>
public class CombatStateService
{
    private readonly List<Monster> _monsters = new();
    private int _nextMonsterId = 1;
    private readonly Random _random = new();

    /// <summary>
    /// Event raised when combat state changes.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Selected party for the current combat.
    /// </summary>
    public Party? SelectedParty { get; private set; }

    /// <summary>
    /// Gets all monsters added to the combat.
    /// </summary>
    public IReadOnlyList<Monster> GetMonsters() => _monsters.AsReadOnly();

    /// <summary>
    /// Gets all combatants (party members + monsters) with their initiative values.
    /// </summary>
    public Dictionary<string, CombatantSetupData> Combatants { get; private set; } = new();

    /// <summary>
    /// Selects a party for the combat.
    /// </summary>
    public void SelectParty(Party? party)
    {
        SelectedParty = party;
        RebuildCombatants();
        NotifyStateChanged();
    }

    /// <summary>
    /// Adds a monster to the combat.
    /// </summary>
    public Monster AddMonster(Monster monster)
    {
        monster.Id = _nextMonsterId++;
        _monsters.Add(monster);
        RebuildCombatants();
        NotifyStateChanged();
        return monster;
    }

    /// <summary>
    /// Removes a monster from the combat.
    /// </summary>
    public void RemoveMonster(int monsterId)
    {
        var monster = _monsters.FirstOrDefault(m => m.Id == monsterId);
        if (monster != null)
        {
            _monsters.Remove(monster);
            RebuildCombatants();
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Rolls initiative for all combatants using 1d20 + modifier.
    /// </summary>
    public void RollInitiativeForAll()
    {
        foreach (var combatant in Combatants.Values)
        {
            combatant.Initiative = RollD20() + combatant.InitiativeModifier;
        }
        NotifyStateChanged();
    }

    /// <summary>
    /// Sets initiative for a specific combatant.
    /// </summary>
    public void SetInitiative(string combatantKey, int initiative)
    {
        if (Combatants.ContainsKey(combatantKey))
        {
            Combatants[combatantKey].Initiative = initiative;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Validates that the combat setup is ready to start.
    /// </summary>
    public bool IsValidForCombat()
    {
        return Combatants.Count > 0;
    }

    /// <summary>
    /// Resets the combat setup.
    /// </summary>
    public void Reset()
    {
        SelectedParty = null;
        _monsters.Clear();
        Combatants.Clear();
        NotifyStateChanged();
    }

    private void RebuildCombatants()
    {
        Combatants.Clear();

        // Add party members
        if (SelectedParty != null)
        {
            foreach (var character in SelectedParty.Characters)
            {
                var key = $"character-{character.Id}";
                Combatants[key] = new CombatantSetupData
                {
                    Name = character.Name,
                    Type = "Character",
                    HpMax = character.HpMax,
                    HpCurrent = character.HpCurrent,
                    AC = character.AC,
                    InitiativeModifier = character.InitiativeModifier,
                    Initiative = 0,
                    ReferenceId = character.Id,
                    IsCharacter = true
                };
            }
        }

        // Add monsters
        foreach (var monster in _monsters)
        {
            var key = $"monster-{monster.Id}";
            Combatants[key] = new CombatantSetupData
            {
                Name = monster.Name,
                Type = monster.Type,
                HpMax = monster.Hp,
                HpCurrent = monster.Hp,
                AC = monster.AC,
                InitiativeModifier = monster.InitiativeModifier,
                Initiative = 0,
                ReferenceId = monster.Id,
                IsCharacter = false
            };
        }
    }

    private int RollD20()
    {
        return _random.Next(1, 21);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}

/// <summary>
/// Data structure for a combatant during setup phase.
/// </summary>
public class CombatantSetupData
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int HpMax { get; set; }
    public int HpCurrent { get; set; }
    public int AC { get; set; }
    public int InitiativeModifier { get; set; }
    public int Initiative { get; set; }
    public int ReferenceId { get; set; }
    public bool IsCharacter { get; set; }
}
