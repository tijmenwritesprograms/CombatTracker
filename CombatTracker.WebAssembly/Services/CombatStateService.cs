using CombatTracker.WebAssembly.Models;
using Microsoft.Extensions.Logging;

namespace CombatTracker.WebAssembly.Services;

/// <summary>
/// In-memory state management service for combat encounters with local storage persistence.
/// </summary>
public class CombatStateService
{
    private readonly List<Monster> _monsters = new();
    private int _nextMonsterId = 1;
    private readonly Random _random = new();
    private readonly StorageStateService? _storageService;
    private readonly ILogger<CombatStateService> _logger;
    private bool _isInitialized = false;

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
    /// Gets a monster by its ID.
    /// </summary>
    /// <param name="monsterId">The ID of the monster to retrieve.</param>
    /// <returns>The monster with the specified ID, or null if not found.</returns>
    public Monster? GetMonsterById(int monsterId) => _monsters.FirstOrDefault(m => m.Id == monsterId);

    /// <summary>
    /// Gets all combatants (party members + monsters) with their initiative values.
    /// </summary>
    public Dictionary<string, CombatantSetupData> Combatants { get; private set; } = new();

    /// <summary>
    /// Active combat instance (null if combat not started).
    /// </summary>
    public Combat? ActiveCombat { get; private set; }

    /// <summary>
    /// Combat log entries for the active combat.
    /// </summary>
    public List<CombatLogEntry> CombatLog { get; private set; } = new();

    /// <summary>
    /// Mapping of combatant setup keys to combat instance indices.
    /// </summary>
    private Dictionary<string, int> _combatantKeyMapping = new();

    /// <summary>
    /// Gets whether combat is currently active.
    /// </summary>
    public bool IsCombatActive => ActiveCombat != null;

    public CombatStateService(ILogger<CombatStateService> logger, StorageStateService? storageService = null)
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
    /// Selects a party for the combat.
    /// </summary>
    public void SelectParty(Party? party)
    {
        SelectedParty = party;
        RebuildCombatants();
        _ = SaveToStorageAsync(); // Fire and forget
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
        _ = SaveToStorageAsync(); // Fire and forget
        NotifyStateChanged();
        return monster;
    }

    /// <summary>
    /// Adds multiple instances of a monster to the combat.
    /// All instances share characteristics and initiative but have individual HP.
    /// </summary>
    /// <param name="baseMonster">The monster template to create instances from.</param>
    /// <param name="count">Number of instances to create.</param>
    /// <returns>List of created monster instances.</returns>
    public List<Monster> AddMonsters(Monster baseMonster, int count)
    {
        if (count <= 0)
        {
            count = 1;
        }

        var createdMonsters = new List<Monster>();
        var groupId = _nextMonsterId; // Use the first monster's ID as the group ID

        for (int i = 0; i < count; i++)
        {
            var instance = CloneMonster(baseMonster);
            instance.Id = _nextMonsterId++;
            instance.GroupId = groupId;
            instance.InstanceNumber = i + 1;
            _monsters.Add(instance);
            createdMonsters.Add(instance);
        }

        RebuildCombatants();
        _ = SaveToStorageAsync(); // Fire and forget
        NotifyStateChanged();
        return createdMonsters;
    }

    /// <summary>
    /// Creates a deep clone of a monster.
    /// </summary>
    private Monster CloneMonster(Monster src)
    {
        return new Monster
        {
            Name = src.Name,
            Size = src.Size,
            Type = src.Type,
            Subtype = src.Subtype,
            Alignment = src.Alignment,
            AC = src.AC,
            ArmorType = src.ArmorType,
            Hp = src.Hp,
            HpFormula = src.HpFormula,
            Speed = src.Speed,
            FlySpeed = src.FlySpeed,
            SwimSpeed = src.SwimSpeed,
            ClimbSpeed = src.ClimbSpeed,
            BurrowSpeed = src.BurrowSpeed,
            Abilities = new AbilityScores
            {
                Strength = src.Abilities.Strength,
                Dexterity = src.Abilities.Dexterity,
                Constitution = src.Abilities.Constitution,
                Intelligence = src.Abilities.Intelligence,
                Wisdom = src.Abilities.Wisdom,
                Charisma = src.Abilities.Charisma
            },
            Skills = src.Skills,
            SavingThrows = src.SavingThrows,
            Vulnerabilities = src.Vulnerabilities,
            Resistances = src.Resistances,
            Immunities = src.Immunities,
            ConditionImmunities = src.ConditionImmunities,
            Senses = src.Senses,
            Languages = src.Languages,
            ChallengeRating = src.ChallengeRating,
            ExperiencePoints = src.ExperiencePoints,
            ProficiencyBonus = src.ProficiencyBonus,
            InitiativeModifier = src.InitiativeModifier,
            Traits = src.Traits?.Select(t => new MonsterTrait { Name = t.Name, Description = t.Description }).ToList() ?? new List<MonsterTrait>(),
            Actions = src.Actions?.Select(a => new MonsterAction
            {
                Name = a.Name,
                Description = a.Description,
                AttackType = a.AttackType,
                AttackBonus = a.AttackBonus,
                Reach = a.Reach,
                Range = a.Range,
                DamageFormula = a.DamageFormula,
                DamageType = a.DamageType,
                AdditionalDamageFormula = a.AdditionalDamageFormula,
                AdditionalDamageType = a.AdditionalDamageType,
                LegendaryActionCost = a.LegendaryActionCost
            }).ToList() ?? new List<MonsterAction>(),
            BonusActions = src.BonusActions?.Select(a => new MonsterAction
            {
                Name = a.Name,
                Description = a.Description,
                AttackType = a.AttackType,
                AttackBonus = a.AttackBonus,
                Reach = a.Reach,
                Range = a.Range,
                DamageFormula = a.DamageFormula,
                DamageType = a.DamageType
            }).ToList() ?? new List<MonsterAction>(),
            Reactions = src.Reactions?.Select(a => new MonsterAction
            {
                Name = a.Name,
                Description = a.Description,
                AttackType = a.AttackType,
                AttackBonus = a.AttackBonus,
                Reach = a.Reach,
                Range = a.Range,
                DamageFormula = a.DamageFormula,
                DamageType = a.DamageType
            }).ToList() ?? new List<MonsterAction>(),
            LegendaryActions = src.LegendaryActions?.Select(a => new MonsterAction
            {
                Name = a.Name,
                Description = a.Description,
                AttackType = a.AttackType,
                AttackBonus = a.AttackBonus,
                Reach = a.Reach,
                Range = a.Range,
                DamageFormula = a.DamageFormula,
                DamageType = a.DamageType,
                LegendaryActionCost = a.LegendaryActionCost
            }).ToList() ?? new List<MonsterAction>(),
            LegendaryActionsPerRound = src.LegendaryActionsPerRound,
            LairActions = src.LairActions?.Select(a => new MonsterAction
            {
                Name = a.Name,
                Description = a.Description,
                AttackType = a.AttackType,
                AttackBonus = a.AttackBonus,
                Reach = a.Reach,
                Range = a.Range,
                DamageFormula = a.DamageFormula,
                DamageType = a.DamageType
            }).ToList() ?? new List<MonsterAction>()
        };
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
            _ = SaveToStorageAsync(); // Fire and forget
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Rolls initiative for all combatants using 1d20 + modifier.
    /// Monsters in the same group share the same initiative roll.
    /// </summary>
    public void RollInitiativeForAll()
    {
        var groupInitiatives = new Dictionary<int, int>();

        foreach (var combatant in Combatants.Values)
        {
            if (combatant.GroupId.HasValue)
            {
                // Use cached group initiative if already rolled
                if (!groupInitiatives.ContainsKey(combatant.GroupId.Value))
                {
                    groupInitiatives[combatant.GroupId.Value] = RollD20() + combatant.InitiativeModifier;
                }
                combatant.Initiative = groupInitiatives[combatant.GroupId.Value];
            }
            else
            {
                combatant.Initiative = RollD20() + combatant.InitiativeModifier;
            }
        }
        _ = SaveToStorageAsync(); // Fire and forget
        NotifyStateChanged();
    }

    /// <summary>
    /// Rolls initiative only for monsters (non-character combatants).
    /// Monsters in the same group share the same initiative roll.
    /// </summary>
    public void RollInitiativeForMonsters()
    {
        var groupInitiatives = new Dictionary<int, int>();

        foreach (var combatant in Combatants.Values.Where(c => !c.IsCharacter))
        {
            if (combatant.GroupId.HasValue)
            {
                // Use cached group initiative if already rolled
                if (!groupInitiatives.ContainsKey(combatant.GroupId.Value))
                {
                    groupInitiatives[combatant.GroupId.Value] = RollD20() + combatant.InitiativeModifier;
                }
                combatant.Initiative = groupInitiatives[combatant.GroupId.Value];
            }
            else
            {
                combatant.Initiative = RollD20() + combatant.InitiativeModifier;
            }
        }
        _ = SaveToStorageAsync(); // Fire and forget
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
            _ = SaveToStorageAsync(); // Fire and forget
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
        _ = SaveToStorageAsync(); // Fire and forget
        NotifyStateChanged();
    }

    /// <summary>
    /// Starts combat with the current combatants.
    /// </summary>
    public void StartCombat()
    {
        if (!IsValidForCombat())
        {
            return;
        }

        // Create combat instance
        ActiveCombat = new Combat
        {
            Id = 1,
            PartyId = SelectedParty?.Id ?? 0,
            Round = 1,
            TurnIndex = 0,
            Combatants = new List<CombatantInstance>()
        };

        // Convert setup data to combat instances, sorted by initiative (descending)
        var sortedCombatants = Combatants
            .OrderByDescending(c => c.Value.Initiative)
            .ThenBy(c => c.Key)
            .ToList();

        _combatantKeyMapping.Clear();
        int index = 0;
        foreach (var kvp in sortedCombatants)
        {
            var combatant = kvp.Value;
            ActiveCombat.Combatants.Add(new CombatantInstance
            {
                Index = index,
                ReferenceId = combatant.ReferenceId,
                GroupId = combatant.GroupId,
                InstanceNumber = combatant.InstanceNumber,
                Initiative = combatant.Initiative,
                HpCurrent = combatant.HpCurrent,
                Status = Status.Alive
            });
            _combatantKeyMapping[kvp.Key] = index;
            index++;
        }

        // Initialize combat log
        CombatLog.Clear();
        AddLogEntry("Turn", $"Combat started! Round {ActiveCombat.Round}");
        
        // Log first turn
        if (ActiveCombat.Combatants.Count > 0)
        {
            var firstCombatant = GetCurrentCombatantData();
            if (firstCombatant != null)
            {
                AddLogEntry("Turn", $"{firstCombatant.Name}'s turn (Initiative: {GetCurrentCombatantInstance()?.Initiative})");
            }
        }

        _ = SaveToStorageAsync(); // Fire and forget
        NotifyStateChanged();
    }

    /// <summary>
    /// Gets the current active combatant instance.
    /// </summary>
    public CombatantInstance? GetCurrentCombatantInstance()
    {
        if (ActiveCombat == null || ActiveCombat.Combatants.Count == 0)
        {
            return null;
        }

        return ActiveCombat.Combatants[ActiveCombat.TurnIndex];
    }

    /// <summary>
    /// Gets the setup data for the current combatant.
    /// </summary>
    public CombatantSetupData? GetCurrentCombatantData()
    {
        var instance = GetCurrentCombatantInstance();
        if (instance == null || ActiveCombat == null)
        {
            return null;
        }

        // Find the matching setup data using the mapping
        var key = _combatantKeyMapping.FirstOrDefault(kvp => kvp.Value == ActiveCombat.TurnIndex).Key;
        if (key != null && Combatants.ContainsKey(key))
        {
            return Combatants[key];
        }

        return null;
    }

    /// <summary>
    /// Gets all combatants as a list with setup data, sorted by initiative order.
    /// </summary>
    public List<(CombatantInstance Instance, CombatantSetupData Data)> GetCombatantsWithData()
    {
        if (ActiveCombat == null)
        {
            return new List<(CombatantInstance, CombatantSetupData)>();
        }

        var result = new List<(CombatantInstance, CombatantSetupData)>();
        
        // Use the mapping to find the corresponding setup data
        foreach (var kvp in _combatantKeyMapping)
        {
            var key = kvp.Key;
            var index = kvp.Value;
            
            if (index < ActiveCombat.Combatants.Count && Combatants.ContainsKey(key))
            {
                result.Add((ActiveCombat.Combatants[index], Combatants[key]));
            }
        }

        return result;
    }

    /// <summary>
    /// Advances to the next turn in combat.
    /// </summary>
    public void NextTurn()
    {
        if (ActiveCombat == null)
        {
            return;
        }

        int nextIndex = ActiveCombat.TurnIndex + 1;
        
        // Check if we've completed a round
        if (nextIndex >= ActiveCombat.Combatants.Count)
        {
            nextIndex = 0;
            ActiveCombat.Round++;
            AddLogEntry("Turn", $"Round {ActiveCombat.Round} begins");
        }

        // Skip dead/unconscious combatants
        int attempts = 0;
        while (attempts < ActiveCombat.Combatants.Count)
        {
            var combatant = ActiveCombat.Combatants[nextIndex];
            if (combatant.Status == Status.Alive)
            {
                break;
            }

            nextIndex++;
            if (nextIndex >= ActiveCombat.Combatants.Count)
            {
                nextIndex = 0;
                ActiveCombat.Round++;
                AddLogEntry("Turn", $"Round {ActiveCombat.Round} begins");
            }
            attempts++;
        }

        ActiveCombat.TurnIndex = nextIndex;

        // Log the new turn
        var currentCombatant = GetCurrentCombatantData();
        if (currentCombatant != null)
        {
            AddLogEntry("Turn", $"{currentCombatant.Name}'s turn");
        }

        _ = SaveToStorageAsync(); // Fire and forget
        NotifyStateChanged();
    }

    /// <summary>
    /// Goes back to the previous turn in combat.
    /// </summary>
    public void PreviousTurn()
    {
        if (ActiveCombat == null)
        {
            return;
        }

        int prevIndex = ActiveCombat.TurnIndex - 1;
        
        // Check if we need to go to previous round
        if (prevIndex < 0)
        {
            if (ActiveCombat.Round > 1)
            {
                ActiveCombat.Round--;
                prevIndex = ActiveCombat.Combatants.Count - 1;
                AddLogEntry("Turn", $"Back to Round {ActiveCombat.Round}");
            }
            else
            {
                prevIndex = 0;
            }
        }

        // Skip dead/unconscious combatants (going backwards)
        int attempts = 0;
        while (attempts < ActiveCombat.Combatants.Count)
        {
            var combatant = ActiveCombat.Combatants[prevIndex];
            if (combatant.Status == Status.Alive)
            {
                break;
            }

            prevIndex--;
            if (prevIndex < 0)
            {
                if (ActiveCombat.Round > 1)
                {
                    ActiveCombat.Round--;
                    prevIndex = ActiveCombat.Combatants.Count - 1;
                    AddLogEntry("Turn", $"Back to Round {ActiveCombat.Round}");
                }
                else
                {
                    prevIndex = 0;
                    break;
                }
            }
            attempts++;
        }

        ActiveCombat.TurnIndex = prevIndex;

        // Log the turn change
        var currentCombatant = GetCurrentCombatantData();
        if (currentCombatant != null)
        {
            AddLogEntry("Turn", $"Back to {currentCombatant.Name}'s turn");
        }

        _ = SaveToStorageAsync(); // Fire and forget
        NotifyStateChanged();
    }

    /// <summary>
    /// Applies damage to a specific combatant.
    /// </summary>
    public void ApplyDamage(int combatantIndex, int damage)
    {
        if (ActiveCombat == null || combatantIndex < 0 || combatantIndex > ActiveCombat.Combatants.Count)
        {
            return;
        }

        var combatant = ActiveCombat.Combatants[combatantIndex];
        
        // Find the matching setup data using the mapping
        var key = _combatantKeyMapping.FirstOrDefault(kvp => kvp.Value == combatantIndex).Key;
        var data = key != null && Combatants.ContainsKey(key) ? Combatants[key] : null;
        
        if (data == null)
        {
            return;
        }

        int oldHp = combatant.HpCurrent;
        combatant.HpCurrent = Math.Max(0, combatant.HpCurrent - damage);

        // Update status based on HP
        var oldStatus = combatant.Status;
        if (combatant.HpCurrent <= 0)
        {
            combatant.Status = Status.Unconscious;
        }

        // Log the damage
        AddLogEntry("Damage", $"{data.Name} takes {damage} damage ({oldHp} → {combatant.HpCurrent} HP)");
        
        // Log status change
        if (oldStatus != combatant.Status)
        {
            AddLogEntry("Status", $"{data.Name} is now {combatant.Status}!");
        }

        _ = SaveToStorageAsync(); // Fire and forget
        NotifyStateChanged();
    }

    /// <summary>
    /// Heals a specific combatant.
    /// </summary>
    public void ApplyHealing(int combatantIndex, int healing)
    {
        if (ActiveCombat == null || combatantIndex < 0 || combatantIndex >= ActiveCombat.Combatants.Count)
        {
            return;
        }

        var combatant = ActiveCombat.Combatants[combatantIndex];
        
        // Find the matching setup data using the mapping
        var key = _combatantKeyMapping.FirstOrDefault(kvp => kvp.Value == combatantIndex).Key;
        var data = key != null && Combatants.ContainsKey(key) ? Combatants[key] : null;
        
        if (data == null)
        {
            return;
        }

        int oldHp = combatant.HpCurrent;
        combatant.HpCurrent = Math.Min(data.HpMax, combatant.HpCurrent + healing);

        // Update status if healed from unconscious
        var oldStatus = combatant.Status;
        if (combatant.HpCurrent > 0 && combatant.Status == Status.Unconscious)
        {
            combatant.Status = Status.Alive;
        }

        // Log the healing
        AddLogEntry("Heal", $"{data.Name} heals {healing} HP ({oldHp} → {combatant.HpCurrent} HP)");
        
        // Log status change
        if (oldStatus != combatant.Status)
        {
            AddLogEntry("Status", $"{data.Name} is now {combatant.Status}!");
        }

        _ = SaveToStorageAsync(); // Fire and forget
        NotifyStateChanged();
    }

    /// <summary>
    /// Ends the current combat and resets to setup mode.
    /// </summary>
    public void EndCombat()
    {
        ActiveCombat = null;
        CombatLog.Clear();
        _ = SaveToStorageAsync(); // Fire and forget
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
            var displayName = monster.InstanceNumber.HasValue
                ? $"{monster.Name} {monster.InstanceNumber}"
                : monster.Name;

            Combatants[key] = new CombatantSetupData
            {
                Name = displayName,
                Type = monster.Type,
                HpMax = monster.Hp,
                HpCurrent = monster.Hp,
                AC = monster.AC,
                InitiativeModifier = monster.InitiativeModifier,
                Initiative = 0,
                ReferenceId = monster.Id,
                IsCharacter = false,
                GroupId = monster.GroupId,
                InstanceNumber = monster.InstanceNumber
            };
        }
    }

    private int RollD20()
    {
        return _random.Next(1, 21);
    }

    private void AddLogEntry(string type, string message)
    {
        if (ActiveCombat == null)
        {
            return;
        }

        CombatLog.Add(new CombatLogEntry
        {
            Round = ActiveCombat.Round,
            TurnIndex = ActiveCombat.TurnIndex,
            Timestamp = DateTime.Now,
            Type = type,
            Message = message
        });
    }

    /// <summary>
    /// Loads combat state from storage.
    /// </summary>
    private async Task LoadFromStorageAsync()
    {
        if (_storageService == null)
        {
            return;
        }

        try
        {
            var data = await _storageService.LoadCombatStateAsync();
            if (data != null)
            {
                _monsters.Clear();
                _monsters.AddRange(data.Monsters);
                _nextMonsterId = data.NextMonsterId;
                SelectedParty = data.SelectedParty;
                Combatants = data.Combatants;
                ActiveCombat = data.ActiveCombat;
                CombatLog = data.CombatLog;
                _combatantKeyMapping = data.CombatantKeyMapping;
                _logger.LogInformation("Loaded combat state from storage");
                NotifyStateChanged();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading combat state from storage");
        }
    }

    /// <summary>
    /// Saves combat state to storage.
    /// </summary>
    private async Task SaveToStorageAsync()
    {
        if (_storageService == null)
        {
            return;
        }

        try
        {
            CombatStorageData? data = null;
            
            // Only save if there's actual combat data
            if (ActiveCombat != null || _monsters.Count > 0 || SelectedParty != null)
            {
                data = new CombatStorageData
                {
                    ActiveCombat = ActiveCombat,
                    CombatLog = CombatLog,
                    Combatants = Combatants,
                    Monsters = _monsters.ToList(),
                    SelectedParty = SelectedParty,
                    NextMonsterId = _nextMonsterId,
                    CombatantKeyMapping = _combatantKeyMapping
                };
            }

            await _storageService.SaveCombatStateAsync(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving combat state to storage");
        }
    }

    /// <summary>
    /// Gets internal state for export/import.
    /// </summary>
    public CombatStorageData? GetInternalState()
    {
        if (ActiveCombat == null && _monsters.Count == 0 && SelectedParty == null)
        {
            return null;
        }

        return new CombatStorageData
        {
            ActiveCombat = ActiveCombat,
            CombatLog = CombatLog,
            Combatants = Combatants,
            Monsters = _monsters.ToList(),
            SelectedParty = SelectedParty,
            NextMonsterId = _nextMonsterId,
            CombatantKeyMapping = _combatantKeyMapping
        };
    }

    /// <summary>
    /// Restores internal state from import.
    /// </summary>
    public async Task RestoreInternalStateAsync(CombatStorageData? data)
    {
        if (data == null)
        {
            _monsters.Clear();
            _nextMonsterId = 1;
            SelectedParty = null;
            Combatants = new Dictionary<string, CombatantSetupData>();
            ActiveCombat = null;
            CombatLog = new List<CombatLogEntry>();
            _combatantKeyMapping = new Dictionary<string, int>();
        }
        else
        {
            _monsters.Clear();
            _monsters.AddRange(data.Monsters);
            _nextMonsterId = data.NextMonsterId;
            SelectedParty = data.SelectedParty;
            Combatants = data.Combatants;
            ActiveCombat = data.ActiveCombat;
            CombatLog = data.CombatLog;
            _combatantKeyMapping = data.CombatantKeyMapping;
        }

        await SaveToStorageAsync();
        NotifyStateChanged();
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
    public int? GroupId { get; set; }
    public int? InstanceNumber { get; set; }
}
