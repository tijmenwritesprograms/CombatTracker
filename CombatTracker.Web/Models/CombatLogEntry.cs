namespace CombatTracker.Web.Models;

/// <summary>
/// Represents a single entry in the combat log.
/// </summary>
public class CombatLogEntry
{
    /// <summary>
    /// Round number when the entry was created.
    /// </summary>
    public int Round { get; set; }

    /// <summary>
    /// Turn index when the entry was created.
    /// </summary>
    public int TurnIndex { get; set; }

    /// <summary>
    /// Timestamp when the entry was created.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Message describing the action or event.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Type of log entry (e.g., "Turn", "Damage", "Heal", "Status").
    /// </summary>
    public string Type { get; set; } = string.Empty;
}
