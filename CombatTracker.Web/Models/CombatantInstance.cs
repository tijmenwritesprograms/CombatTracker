using System.ComponentModel.DataAnnotations;

namespace CombatTracker.Web.Models;

/// <summary>
/// Represents an instance of a combatant (character or monster) in a specific combat encounter.
/// </summary>
public class CombatantInstance
{
    /// <summary>
    /// Reference to the original character or monster ID.
    /// </summary>
    [Required]
    public int ReferenceId { get; set; }

    /// <summary>
    /// Initiative roll result for turn order.
    /// </summary>
    [Range(1, 30)]
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
