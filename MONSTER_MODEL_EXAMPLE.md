# Monster Model - Extended D&D 5e Statblock Support

The Monster model has been extended to support all properties from a standard D&D 5e statblock. This document provides an example of how to create a Monster instance with all available properties.

## Example: Creating an Orc

Based on the standard D&D 5e Orc statblock:

```
Orc
Medium Humanoid (Orc), Chaotic Evil

Armor Class 13 (hide armor)
Hit Points 15 (2d8 + 6)
Speed 30 ft.

STR     DEX     CON     INT     WIS     CHA
16 (+3) 12 (+1) 16 (+3) 7 (-2)  11 (+0) 10 (+0)

Skills Intimidation +2
Senses Darkvision 60 ft., Passive Perception 10
Languages Common, Orc
Challenge 1/2 (100 XP)
Proficiency Bonus +2

Traits
Aggressive. As a bonus action, the orc can move up to its speed toward a hostile creature that it can see.

Actions
Greataxe. Melee Weapon Attack: +5 to hit, reach 5 ft., one target. Hit: 9 (1d12 + 3) slashing damage.
Javelin. Melee or Ranged Weapon Attack: +5 to hit, reach 5 ft. or range 30/120 ft., one target. Hit: 6 (1d6 + 3) piercing damage.
```

### C# Code Example

```csharp
using CombatTracker.WebAssembly.Models;

var orc = new Monster
{
    // Basic Information
    Name = "Orc",
    Size = "Medium",
    Type = "Humanoid",
    Subtype = "Orc",
    Alignment = "Chaotic Evil",
    
    // Defense
    AC = 13,
    ArmorType = "hide armor",
    Hp = 15,
    HpFormula = "2d8 + 6",
    
    // Movement
    Speed = 30,
    FlySpeed = 0,
    SwimSpeed = 0,
    ClimbSpeed = 0,
    BurrowSpeed = 0,
    
    // Ability Scores
    Abilities = new AbilityScores
    {
        Strength = 16,      // +3 modifier
        Dexterity = 12,     // +1 modifier
        Constitution = 16,  // +3 modifier
        Intelligence = 7,   // -2 modifier
        Wisdom = 11,        // +0 modifier
        Charisma = 10       // +0 modifier
    },
    
    // Skills and Proficiencies
    Skills = "Intimidation +2",
    SavingThrows = null, // Orcs don't have saving throw proficiencies
    
    // Resistances and Immunities
    Vulnerabilities = null,
    Resistances = null,
    Immunities = null,
    ConditionImmunities = null,
    
    // Senses and Languages
    Senses = "Darkvision 60 ft., Passive Perception 10",
    Languages = "Common, Orc",
    
    // Challenge and Experience
    ChallengeRating = "1/2",
    ExperiencePoints = 100,
    ProficiencyBonus = 2,
    
    // Initiative (typically Dexterity modifier)
    InitiativeModifier = 1,
    
    // Traits
    Traits = new List<MonsterTrait>
    {
        new MonsterTrait
        {
            Name = "Aggressive",
            Description = "As a bonus action, the orc can move up to its speed toward a hostile creature that it can see."
        }
    },
    
    // Actions
    Actions = new List<MonsterAction>
    {
        new MonsterAction
        {
            Name = "Greataxe",
            Description = "Melee Weapon Attack: +5 to hit, reach 5 ft., one target. Hit: 9 (1d12 + 3) slashing damage.",
            AttackType = "Melee Weapon Attack",
            AttackBonus = 5,
            Reach = 5,
            DamageFormula = "1d12 + 3",
            DamageType = "slashing"
        },
        new MonsterAction
        {
            Name = "Javelin",
            Description = "Melee or Ranged Weapon Attack: +5 to hit, reach 5 ft. or range 30/120 ft., one target. Hit: 6 (1d6 + 3) piercing damage.",
            AttackType = "Melee or Ranged Weapon Attack",
            AttackBonus = 5,
            Reach = 5,
            Range = "30/120",
            DamageFormula = "1d6 + 3",
            DamageType = "piercing"
        }
    },
    
    // Bonus Actions, Reactions, Legendary Actions, Lair Actions
    BonusActions = new List<MonsterAction>(),
    Reactions = new List<MonsterAction>(),
    LegendaryActions = new List<MonsterAction>(),
    LegendaryActionsPerRound = 0,
    LairActions = new List<MonsterAction>()
};

// Ability score modifiers are automatically calculated
Console.WriteLine($"STR modifier: {orc.Abilities.StrengthModifier}");     // Output: 3
Console.WriteLine($"DEX modifier: {orc.Abilities.DexterityModifier}");     // Output: 1
Console.WriteLine($"CON modifier: {orc.Abilities.ConstitutionModifier}");  // Output: 3
Console.WriteLine($"INT modifier: {orc.Abilities.IntelligenceModifier}");  // Output: -2
Console.WriteLine($"WIS modifier: {orc.Abilities.WisdomModifier}");        // Output: 0
Console.WriteLine($"CHA modifier: {orc.Abilities.CharismaModifier}");      // Output: 0
```

## Model Properties Reference

### Basic Information
- `Name` (required): The monster's name
- `Size`: Tiny, Small, Medium, Large, Huge, or Gargantuan
- `Type` (required): Humanoid, Beast, Dragon, Undead, etc.
- `Subtype`: Specific creature subtype (e.g., Orc, Goblinoid)
- `Alignment`: Alignment (e.g., Chaotic Evil, Lawful Good)

### Defense
- `AC`: Armor Class
- `ArmorType`: Description of armor (e.g., "natural armor", "plate mail")
- `Hp`: Maximum hit points
- `HpFormula`: Hit dice formula (e.g., "2d8 + 6")

### Movement
- `Speed`: Walking speed in feet
- `FlySpeed`: Flying speed (0 if cannot fly)
- `SwimSpeed`: Swimming speed (0 if no special swim speed)
- `ClimbSpeed`: Climbing speed (0 if no special climb speed)
- `BurrowSpeed`: Burrowing speed (0 if cannot burrow)

### Ability Scores
- `Abilities`: AbilityScores object with:
  - `Strength`, `Dexterity`, `Constitution`, `Intelligence`, `Wisdom`, `Charisma`
  - Auto-calculated modifiers: `StrengthModifier`, `DexterityModifier`, etc.

### Proficiencies
- `SavingThrows`: Saving throw proficiencies (e.g., "Str +5, Dex +3")
- `Skills`: Skill proficiencies (e.g., "Perception +4, Stealth +6")

### Resistances and Immunities
- `Vulnerabilities`: Damage vulnerabilities
- `Resistances`: Damage resistances
- `Immunities`: Damage immunities
- `ConditionImmunities`: Condition immunities

### Senses and Languages
- `Senses`: Special senses (e.g., "Darkvision 60 ft.")
- `Languages`: Languages spoken/understood

### Challenge
- `ChallengeRating`: CR as a string (e.g., "1/2", "1", "30")
- `ExperiencePoints`: XP awarded
- `ProficiencyBonus`: Proficiency bonus

### Combat
- `InitiativeModifier`: Initiative modifier
- `Traits`: List of special traits/abilities
- `Actions`: List of actions
- `BonusActions`: List of bonus actions
- `Reactions`: List of reactions
- `LegendaryActions`: List of legendary actions
- `LegendaryActionsPerRound`: Number of legendary actions per round
- `LairActions`: List of lair actions

## MonsterTrait Properties
- `Name`: Trait name
- `Description`: What the trait does

## MonsterAction Properties
- `Name`: Action name
- `Description`: Full action description
- `AttackType`: Type of attack (optional)
- `AttackBonus`: Attack bonus to hit (optional)
- `Reach`: Reach in feet (optional)
- `Range`: Range in feet (optional, e.g., "30/120")
- `DamageFormula`: Damage dice formula (optional)
- `DamageType`: Damage type (optional)
- `AdditionalDamageFormula`: Additional damage (optional)
- `AdditionalDamageType`: Additional damage type (optional)
- `LegendaryActionCost`: Cost in legendary actions (0 for regular actions)

## Legacy Support

The original `Attacks` property is still available but marked as obsolete. Use the `Actions` property instead for full D&D 5e support.

## AI Integration

The extended Monster model is designed to work with AI-powered statblock parsing. The `/api/parse-statblock` endpoint will extract all these properties from pasted D&D 5e statblocks and return a fully populated Monster object.
