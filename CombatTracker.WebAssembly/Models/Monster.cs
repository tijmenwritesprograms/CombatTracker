using System.ComponentModel.DataAnnotations;

namespace CombatTracker.WebAssembly.Models;

/// <summary>
/// Represents a monster or enemy creature in combat with full D&D 5e statblock support.
/// </summary>
public class Monster
{
    /// <summary>
    /// Unique identifier for the monster.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Group identifier for monsters that share characteristics (like initiative).
    /// Monsters with the same GroupId are instances of the same creature type.
    /// Null if this is a solo monster.
    /// </summary>
    public int? GroupId { get; set; }

    /// <summary>
    /// Instance number within a group (e.g., 1, 2, 3 for "Orc 1", "Orc 2", "Orc 3").
    /// Only set during combat setup when multiple instances are created.
    /// </summary>
    public int? InstanceNumber { get; set; }

    /// <summary>
    /// Name of the monster.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Size of the monster (e.g., "Tiny", "Small", "Medium", "Large", "Huge", "Gargantuan").
    /// </summary>
    [StringLength(20)]
    public string Size { get; set; } = "Medium";

    /// <summary>
    /// Type of the monster (e.g., "Humanoid", "Beast", "Undead", "Dragon").
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Subtype of the monster (e.g., "Orc", "Goblinoid", "Shapechanger").
    /// </summary>
    [StringLength(50)]
    public string? Subtype { get; set; }

    /// <summary>
    /// Alignment (e.g., "Chaotic Evil", "Lawful Good", "Neutral").
    /// </summary>
    [StringLength(50)]
    public string Alignment { get; set; } = "Unaligned";

    /// <summary>
    /// Armor Class (defense rating).
    /// </summary>
    [Range(1, 30)]
    public int AC { get; set; }

    /// <summary>
    /// Description of armor type (e.g., "hide armor", "natural armor", "plate mail").
    /// </summary>
    [StringLength(100)]
    public string? ArmorType { get; set; }

    /// <summary>
    /// Maximum hit points.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Hp { get; set; }

    /// <summary>
    /// Hit points formula (e.g., "2d8 + 6").
    /// </summary>
    [StringLength(50)]
    public string? HpFormula { get; set; }

    /// <summary>
    /// Movement speed in feet.
    /// </summary>
    [Range(0, 200)]
    public int Speed { get; set; } = 30;

    /// <summary>
    /// Flying speed in feet (0 if cannot fly).
    /// </summary>
    [Range(0, 200)]
    public int FlySpeed { get; set; } = 0;

    /// <summary>
    /// Swimming speed in feet (0 if cannot swim faster than walking).
    /// </summary>
    [Range(0, 200)]
    public int SwimSpeed { get; set; } = 0;

    /// <summary>
    /// Climbing speed in feet (0 if cannot climb faster than walking).
    /// </summary>
    [Range(0, 200)]
    public int ClimbSpeed { get; set; } = 0;

    /// <summary>
    /// Burrowing speed in feet (0 if cannot burrow).
    /// </summary>
    [Range(0, 200)]
    public int BurrowSpeed { get; set; } = 0;

    /// <summary>
    /// Ability scores for the monster.
    /// </summary>
    public AbilityScores Abilities { get; set; } = new();

    /// <summary>
    /// Saving throw proficiencies (e.g., "Str +5, Dex +3").
    /// </summary>
    [StringLength(200)]
    public string? SavingThrows { get; set; }

    /// <summary>
    /// Skill proficiencies (e.g., "Perception +4, Stealth +6").
    /// </summary>
    [StringLength(500)]
    public string? Skills { get; set; }

    /// <summary>
    /// Damage vulnerabilities (e.g., "fire, cold").
    /// </summary>
    [StringLength(200)]
    public string? Vulnerabilities { get; set; }

    /// <summary>
    /// Damage resistances (e.g., "bludgeoning, piercing, and slashing from nonmagical attacks").
    /// </summary>
    [StringLength(500)]
    public string? Resistances { get; set; }

    /// <summary>
    /// Damage immunities (e.g., "poison, psychic").
    /// </summary>
    [StringLength(200)]
    public string? Immunities { get; set; }

    /// <summary>
    /// Condition immunities (e.g., "charmed, frightened, poisoned").
    /// </summary>
    [StringLength(200)]
    public string? ConditionImmunities { get; set; }

    /// <summary>
    /// Senses (e.g., "Darkvision 60 ft., Passive Perception 10").
    /// </summary>
    [StringLength(500)]
    public string? Senses { get; set; }

    /// <summary>
    /// Languages the monster can speak and understand.
    /// </summary>
    [StringLength(200)]
    public string? Languages { get; set; }

    /// <summary>
    /// Challenge rating (e.g., "1/2", "1", "5", "30").
    /// </summary>
    [StringLength(10)]
    public string ChallengeRating { get; set; } = "0";

    /// <summary>
    /// Experience points awarded for defeating the monster.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int ExperiencePoints { get; set; } = 0;

    /// <summary>
    /// Proficiency bonus for the monster.
    /// </summary>
    [Range(2, 9)]
    public int ProficiencyBonus { get; set; } = 2;

    /// <summary>
    /// Initiative modifier (added to d20 roll for initiative).
    /// Typically calculated from Dexterity modifier.
    /// </summary>
    [Range(-5, 10)]
    public int InitiativeModifier { get; set; }

    /// <summary>
    /// Special traits and abilities (e.g., "Aggressive", "Pack Tactics", "Regeneration").
    /// </summary>
    public List<MonsterTrait> Traits { get; set; } = new();

    /// <summary>
    /// Actions the monster can take in combat.
    /// </summary>
    public List<MonsterAction> Actions { get; set; } = new();

    /// <summary>
    /// Bonus actions the monster can take.
    /// </summary>
    public List<MonsterAction> BonusActions { get; set; } = new();

    /// <summary>
    /// Reactions the monster can take.
    /// </summary>
    public List<MonsterAction> Reactions { get; set; } = new();

    /// <summary>
    /// Legendary actions (if any).
    /// </summary>
    public List<MonsterAction> LegendaryActions { get; set; } = new();

    /// <summary>
    /// Number of legendary actions the monster can take per round.
    /// </summary>
    [Range(0, 5)]
    public int LegendaryActionsPerRound { get; set; } = 0;

    /// <summary>
    /// Lair actions (if any).
    /// </summary>
    public List<MonsterAction> LairActions { get; set; } = new();

    /// <summary>
    /// Collection of attacks the monster can make (legacy support).
    /// </summary>
    [Obsolete("Use Actions property instead for full action support")]
    public List<Attack> Attacks { get; set; } = new();
}

/// <summary>
/// Represents the six ability scores in D&D 5e.
/// </summary>
public class AbilityScores
{
    /// <summary>
    /// Strength score (1-30).
    /// </summary>
    [Range(1, 30)]
    public int Strength { get; set; } = 10;

    /// <summary>
    /// Dexterity score (1-30).
    /// </summary>
    [Range(1, 30)]
    public int Dexterity { get; set; } = 10;

    /// <summary>
    /// Constitution score (1-30).
    /// </summary>
    [Range(1, 30)]
    public int Constitution { get; set; } = 10;

    /// <summary>
    /// Intelligence score (1-30).
    /// </summary>
    [Range(1, 30)]
    public int Intelligence { get; set; } = 10;

    /// <summary>
    /// Wisdom score (1-30).
    /// </summary>
    [Range(1, 30)]
    public int Wisdom { get; set; } = 10;

    /// <summary>
    /// Charisma score (1-30).
    /// </summary>
    [Range(1, 30)]
    public int Charisma { get; set; } = 10;

    /// <summary>
    /// Calculate the ability modifier from an ability score.
    /// </summary>
    public static int GetModifier(int abilityScore)
    {
        return (abilityScore - 10) / 2;
    }

    /// <summary>
    /// Get the Strength modifier.
    /// </summary>
    public int StrengthModifier => GetModifier(Strength);

    /// <summary>
    /// Get the Dexterity modifier.
    /// </summary>
    public int DexterityModifier => GetModifier(Dexterity);

    /// <summary>
    /// Get the Constitution modifier.
    /// </summary>
    public int ConstitutionModifier => GetModifier(Constitution);

    /// <summary>
    /// Get the Intelligence modifier.
    /// </summary>
    public int IntelligenceModifier => GetModifier(Intelligence);

    /// <summary>
    /// Get the Wisdom modifier.
    /// </summary>
    public int WisdomModifier => GetModifier(Wisdom);

    /// <summary>
    /// Get the Charisma modifier.
    /// </summary>
    public int CharismaModifier => GetModifier(Charisma);
}

/// <summary>
/// Represents a special trait or ability of a monster.
/// </summary>
public class MonsterTrait
{
    /// <summary>
    /// Name of the trait (e.g., "Aggressive", "Pack Tactics").
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what the trait does.
    /// </summary>
    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Represents an action, bonus action, reaction, or legendary action a monster can take.
/// </summary>
public class MonsterAction
{
    /// <summary>
    /// Name of the action (e.g., "Multiattack", "Greataxe", "Fire Breath").
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the action.
    /// </summary>
    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type of attack (if applicable): "Melee Weapon Attack", "Ranged Weapon Attack", "Melee or Ranged Weapon Attack", "Melee Spell Attack", "Ranged Spell Attack".
    /// </summary>
    [StringLength(50)]
    public string? AttackType { get; set; }

    /// <summary>
    /// Attack bonus to hit (if applicable).
    /// </summary>
    public int? AttackBonus { get; set; }

    /// <summary>
    /// Reach in feet (for melee attacks).
    /// </summary>
    public int? Reach { get; set; }

    /// <summary>
    /// Range in feet (for ranged attacks, e.g., "30/120" for normal/long range).
    /// </summary>
    [StringLength(20)]
    public string? Range { get; set; }

    /// <summary>
    /// Damage formula (e.g., "1d12 + 3").
    /// </summary>
    [StringLength(50)]
    public string? DamageFormula { get; set; }

    /// <summary>
    /// Damage type (e.g., "slashing", "piercing", "fire").
    /// </summary>
    [StringLength(50)]
    public string? DamageType { get; set; }

    /// <summary>
    /// Additional damage formula (for versatile weapons or additional effects).
    /// </summary>
    [StringLength(50)]
    public string? AdditionalDamageFormula { get; set; }

    /// <summary>
    /// Additional damage type.
    /// </summary>
    [StringLength(50)]
    public string? AdditionalDamageType { get; set; }

    /// <summary>
    /// Number of legendary action points this action costs (for legendary actions).
    /// </summary>
    [Range(0, 3)]
    public int LegendaryActionCost { get; set; } = 0;
}
