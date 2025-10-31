using System.ComponentModel.DataAnnotations;

namespace CombatTracker.WebAssembly.Models;

/// <summary>
/// Represents a player character in a party.
/// </summary>
public class Character
{
    /// <summary>
    /// Unique identifier for the character.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the character.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Class of the character (e.g., Fighter, Wizard, Rogue).
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Character level (1-20 in D&D 5e).
    /// </summary>
    [Range(1, 20)]
    public int Level { get; set; }

    /// <summary>
    /// Current hit points.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int HpCurrent { get; set; }

    /// <summary>
    /// Maximum hit points.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int HpMax { get; set; }

    /// <summary>
    /// Armor Class (defense rating).
    /// </summary>
    [Range(1, 30)]
    public int AC { get; set; }

    /// <summary>
    /// Initiative modifier (added to d20 roll for initiative).
    /// </summary>
    [Range(-5, 10)]
    public int InitiativeModifier { get; set; }

    /// <summary>
    /// Additional notes about the character.
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }
}
