namespace CombatTracker.Web.Models;

/// <summary>
/// Represents the current status of a combatant in combat.
/// </summary>
public enum Status
{
    /// <summary>
    /// The combatant is alive and can take actions.
    /// </summary>
    Alive,

    /// <summary>
    /// The combatant is unconscious and cannot take actions.
    /// </summary>
    Unconscious,

    /// <summary>
    /// The combatant is dead.
    /// </summary>
    Dead
}
