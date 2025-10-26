using System.ComponentModel.DataAnnotations;

namespace CombatTracker.WebAssembly.Models;

/// <summary>
/// Represents an active combat encounter.
/// </summary>
public class Combat
{
    /// <summary>
    /// Unique identifier for the combat encounter.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Reference to the party participating in this combat.
    /// </summary>
    [Required]
    public int PartyId { get; set; }

    /// <summary>
    /// Current round number (starts at 1).
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Round { get; set; } = 1;

    /// <summary>
    /// Index of the current turn in the initiative order (0-based).
    /// </summary>
    [Range(0, int.MaxValue)]
    public int TurnIndex { get; set; }

    /// <summary>
    /// Collection of all combatants in this encounter.
    /// </summary>
    public List<CombatantInstance> Combatants { get; set; } = new();
}
