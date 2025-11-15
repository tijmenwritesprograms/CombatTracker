using System.ComponentModel.DataAnnotations;

namespace CombatTracker.WebAssembly.Models;

/// <summary>
/// Represents an instance of a combatant (character or monster) in a specific combat encounter.
/// </summary>
public class CombatantInstance
{
    /// <summary>
    /// An combattant index to keep track of the combattant
    /// </summary>
    
    public int Index { get; set; }

    /// <summary>
    /// Reference to the original character or monster ID.
    /// </summary>
    [Required]
    public int ReferenceId { get; set; }

    /// <summary>
    /// Group identifier for monsters that are part of a group.
    /// Null if this is a solo combatant or a character.
    /// </summary>
    public int? GroupId { get; set; }

    /// <summary>
    /// Instance number within a group (e.g., 1, 2, 3 for "Orc 1", "Orc 2", "Orc 3").
    /// Null if this is not part of a group.
    /// </summary>
    public int? InstanceNumber { get; set; }

    /// <summary>
    /// Initiative roll result for turn order.
    /// </summary>
    [Range(-4, 30)]
    public int Initiative { get; set; }

    /// <summary>
    /// Current hit points during this combat.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int HpCurrent { get; set; }

    /// <summary>
    /// Current status of the combatant.
    /// </summary>
    [Required]
    public Status Status { get; set; } = Status.Alive;
}
