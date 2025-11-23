using CombatTracker.WebAssembly.Models;

namespace CombatTracker.WebAssembly.Tests;

/// <summary>
/// Test data builders for creating test objects with sensible defaults
/// </summary>
public static class TestDataBuilders
{
    /// <summary>
    /// Creates a valid Character with default values for testing
    /// </summary>
    public static CharacterBuilder CreateCharacter() => new CharacterBuilder();

    /// <summary>
    /// Creates a valid Monster with default values for testing
    /// </summary>
    public static MonsterBuilder CreateMonster() => new MonsterBuilder();

    /// <summary>
    /// Creates a valid Party with default values for testing
    /// </summary>
    public static PartyBuilder CreateParty() => new PartyBuilder();

    /// <summary>
    /// Creates a valid Combat with default values for testing
    /// </summary>
    public static CombatBuilder CreateCombat() => new CombatBuilder();

    /// <summary>
    /// Creates a valid CombatantInstance with default values for testing
    /// </summary>
    public static CombatantInstanceBuilder CreateCombatantInstance() => new CombatantInstanceBuilder();
}

/// <summary>
/// Builder for Character test data
/// </summary>
public class CharacterBuilder
{
    private int _id = 1;
    private string _name = "Test Character";
    private string _class = "Fighter";
    private int _level = 5;
    private int _hpCurrent = 45;
    private int _hpMax = 50;
    private int _ac = 18;
    private int _initiativeModifier = 2;
    private string? _notes;

    public CharacterBuilder WithId(int id) { _id = id; return this; }
    public CharacterBuilder WithName(string name) { _name = name; return this; }
    public CharacterBuilder WithClass(string className) { _class = className; return this; }
    public CharacterBuilder WithLevel(int level) { _level = level; return this; }
    public CharacterBuilder WithHpCurrent(int hpCurrent) { _hpCurrent = hpCurrent; return this; }
    public CharacterBuilder WithHpMax(int hpMax) { _hpMax = hpMax; return this; }
    public CharacterBuilder WithAC(int ac) { _ac = ac; return this; }
    public CharacterBuilder WithInitiativeModifier(int modifier) { _initiativeModifier = modifier; return this; }
    public CharacterBuilder WithNotes(string notes) { _notes = notes; return this; }

    public Character Build() => new Character
    {
        Id = _id,
        Name = _name,
        Class = _class,
        Level = _level,
        HpCurrent = _hpCurrent,
        HpMax = _hpMax,
        AC = _ac,
        InitiativeModifier = _initiativeModifier,
        Notes = _notes
    };
}

/// <summary>
/// Builder for Monster test data
/// </summary>
public class MonsterBuilder
{
    private int _id = 1;
    private int? _groupId;
    private int? _instanceNumber;
    private string _name = "Test Monster";
    private string _size = "Medium";
    private string _type = "Humanoid";
    private string? _subtype;
    private string _alignment = "Neutral";
    private int _ac = 15;
    private string? _armorType;
    private int _hp = 50;
    private string? _hpFormula = "5d8 + 25";
    private int _speed = 30;
    private int _initiativeModifier = 2;
    private string _challengeRating = "1";
    private int _experiencePoints = 200;
    private int _proficiencyBonus = 2;
    private AbilityScores _abilities = new AbilityScores();
    private List<MonsterTrait> _traits = new();
    private List<MonsterAction> _actions = new();

    public MonsterBuilder WithId(int id) { _id = id; return this; }
    public MonsterBuilder WithGroupId(int? groupId) { _groupId = groupId; return this; }
    public MonsterBuilder WithInstanceNumber(int? instanceNumber) { _instanceNumber = instanceNumber; return this; }
    public MonsterBuilder WithName(string name) { _name = name; return this; }
    public MonsterBuilder WithSize(string size) { _size = size; return this; }
    public MonsterBuilder WithType(string type) { _type = type; return this; }
    public MonsterBuilder WithSubtype(string subtype) { _subtype = subtype; return this; }
    public MonsterBuilder WithAlignment(string alignment) { _alignment = alignment; return this; }
    public MonsterBuilder WithAC(int ac) { _ac = ac; return this; }
    public MonsterBuilder WithArmorType(string armorType) { _armorType = armorType; return this; }
    public MonsterBuilder WithHp(int hp) { _hp = hp; return this; }
    public MonsterBuilder WithHpFormula(string hpFormula) { _hpFormula = hpFormula; return this; }
    public MonsterBuilder WithSpeed(int speed) { _speed = speed; return this; }
    public MonsterBuilder WithInitiativeModifier(int modifier) { _initiativeModifier = modifier; return this; }
    public MonsterBuilder WithChallengeRating(string cr) { _challengeRating = cr; return this; }
    public MonsterBuilder WithExperiencePoints(int xp) { _experiencePoints = xp; return this; }
    public MonsterBuilder WithProficiencyBonus(int bonus) { _proficiencyBonus = bonus; return this; }
    public MonsterBuilder WithAbilityScores(AbilityScores abilities) { _abilities = abilities; return this; }
    public MonsterBuilder WithTraits(List<MonsterTrait> traits) { _traits = traits; return this; }
    public MonsterBuilder WithActions(List<MonsterAction> actions) { _actions = actions; return this; }

    public Monster Build() => new Monster
    {
        Id = _id,
        GroupId = _groupId,
        InstanceNumber = _instanceNumber,
        Name = _name,
        Size = _size,
        Type = _type,
        Subtype = _subtype,
        Alignment = _alignment,
        AC = _ac,
        ArmorType = _armorType,
        Hp = _hp,
        HpFormula = _hpFormula,
        Speed = _speed,
        InitiativeModifier = _initiativeModifier,
        ChallengeRating = _challengeRating,
        ExperiencePoints = _experiencePoints,
        ProficiencyBonus = _proficiencyBonus,
        Abilities = _abilities,
        Traits = _traits,
        Actions = _actions
    };
}

/// <summary>
/// Builder for Party test data
/// </summary>
public class PartyBuilder
{
    private int _id = 1;
    private string _name = "Test Party";
    private List<Character> _characters = new();

    public PartyBuilder WithId(int id) { _id = id; return this; }
    public PartyBuilder WithName(string name) { _name = name; return this; }
    public PartyBuilder WithCharacters(List<Character> characters) { _characters = characters; return this; }
    public PartyBuilder AddCharacter(Character character)
    {
        _characters.Add(character);
        return this;
    }

    public Party Build() => new Party
    {
        Id = _id,
        Name = _name,
        Characters = _characters
    };
}

/// <summary>
/// Builder for Combat test data
/// </summary>
public class CombatBuilder
{
    private int _id = 1;
    private int _partyId = 1;
    private List<CombatantInstance> _combatants = new();
    private int _round = 1;
    private int _turnIndex = 0;

    public CombatBuilder WithId(int id) { _id = id; return this; }
    public CombatBuilder WithPartyId(int partyId) { _partyId = partyId; return this; }
    public CombatBuilder WithCombatants(List<CombatantInstance> combatants) { _combatants = combatants; return this; }
    public CombatBuilder AddCombatant(CombatantInstance combatant)
    {
        _combatants.Add(combatant);
        return this;
    }
    public CombatBuilder WithRound(int round) { _round = round; return this; }
    public CombatBuilder WithTurnIndex(int index) { _turnIndex = index; return this; }

    public Combat Build() => new Combat
    {
        Id = _id,
        PartyId = _partyId,
        Combatants = _combatants,
        Round = _round,
        TurnIndex = _turnIndex
    };
}

/// <summary>
/// Builder for CombatantInstance test data
/// </summary>
public class CombatantInstanceBuilder
{
    private int _index = 0;
    private int _referenceId = 1;
    private int? _groupId;
    private int? _instanceNumber;
    private int _initiative = 10;
    private int _hpCurrent = 50;
    private Status _status = Status.Alive;

    public CombatantInstanceBuilder WithIndex(int index) { _index = index; return this; }
    public CombatantInstanceBuilder WithReferenceId(int referenceId) { _referenceId = referenceId; return this; }
    public CombatantInstanceBuilder WithGroupId(int? groupId) { _groupId = groupId; return this; }
    public CombatantInstanceBuilder WithInstanceNumber(int? instanceNumber) { _instanceNumber = instanceNumber; return this; }
    public CombatantInstanceBuilder WithInitiative(int initiative) { _initiative = initiative; return this; }
    public CombatantInstanceBuilder WithHpCurrent(int hpCurrent) { _hpCurrent = hpCurrent; return this; }
    public CombatantInstanceBuilder WithStatus(Status status) { _status = status; return this; }

    public CombatantInstance Build() => new CombatantInstance
    {
        Index = _index,
        ReferenceId = _referenceId,
        GroupId = _groupId,
        InstanceNumber = _instanceNumber,
        Initiative = _initiative,
        HpCurrent = _hpCurrent,
        Status = _status
    };
}

/// <summary>
/// Common test fixtures for various test scenarios
/// </summary>
public static class TestFixtures
{
    /// <summary>
    /// Creates a standard fighter character
    /// </summary>
    public static Character CreateFighter(string name = "Breaker Stonefist") =>
        TestDataBuilders.CreateCharacter()
            .WithName(name)
            .WithClass("Fighter")
            .WithLevel(5)
            .WithHpMax(50)
            .WithHpCurrent(50)
            .WithAC(18)
            .WithInitiativeModifier(1)
            .Build();

    /// <summary>
    /// Creates a standard wizard character
    /// </summary>
    public static Character CreateWizard(string name = "Eldrid Windspear") =>
        TestDataBuilders.CreateCharacter()
            .WithName(name)
            .WithClass("Wizard")
            .WithLevel(5)
            .WithHpMax(30)
            .WithHpCurrent(30)
            .WithAC(13)
            .WithInitiativeModifier(3)
            .Build();

    /// <summary>
    /// Creates a standard goblin monster
    /// </summary>
    public static Monster CreateGoblin() =>
        TestDataBuilders.CreateMonster()
            .WithName("Goblin")
            .WithSize("Small")
            .WithType("Humanoid")
            .WithSubtype("Goblinoid")
            .WithAlignment("Neutral Evil")
            .WithAC(15)
            .WithArmorType("Leather Armor, Shield")
            .WithHp(7)
            .WithHpFormula("2d6")
            .WithSpeed(30)
            .WithInitiativeModifier(2)
            .WithChallengeRating("1/4")
            .WithExperiencePoints(50)
            .Build();

    /// <summary>
    /// Creates a standard orc monster
    /// </summary>
    public static Monster CreateOrc() =>
        TestDataBuilders.CreateMonster()
            .WithName("Orc")
            .WithSize("Medium")
            .WithType("Humanoid")
            .WithSubtype("Orc")
            .WithAlignment("Chaotic Evil")
            .WithAC(13)
            .WithArmorType("Hide Armor")
            .WithHp(15)
            .WithHpFormula("2d8 + 6")
            .WithSpeed(30)
            .WithInitiativeModifier(1)
            .WithChallengeRating("1/2")
            .WithExperiencePoints(100)
            .Build();

    /// <summary>
    /// Creates a standard party with two characters
    /// </summary>
    public static Party CreateStandardParty() =>
        TestDataBuilders.CreateParty()
            .WithName("The Brave Adventurers")
            .AddCharacter(CreateFighter())
            .AddCharacter(CreateWizard())
            .Build();

    /// <summary>
    /// Creates a combat instance with one player and one monster
    /// </summary>
    public static Combat CreateSimpleCombat()
    {
        var fighter = CreateFighter();
        var goblin = CreateGoblin();

        var playerCombatant = TestDataBuilders.CreateCombatantInstance()
            .WithIndex(0)
            .WithReferenceId(fighter.Id)
            .WithInitiative(15)
            .WithHpCurrent(fighter.HpCurrent)
            .WithStatus(Status.Alive)
            .Build();

        var monsterCombatant = TestDataBuilders.CreateCombatantInstance()
            .WithIndex(1)
            .WithReferenceId(goblin.Id)
            .WithInitiative(12)
            .WithHpCurrent(goblin.Hp)
            .WithStatus(Status.Alive)
            .Build();

        return TestDataBuilders.CreateCombat()
            .WithPartyId(1)
            .AddCombatant(playerCombatant)
            .AddCombatant(monsterCombatant)
            .WithRound(1)
            .WithTurnIndex(0)
            .Build();
    }
}
