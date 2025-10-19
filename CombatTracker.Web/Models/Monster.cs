using System.ComponentModel.DataAnnotations;

namespace CombatTracker.Web.Models;

/// <summary>
/// Represents a monster or enemy creature in combat.
/// </summary>
public class Monster
{
    /// <summary>
    /// Unique identifier for the monster.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the monster.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Type of the monster (e.g., "Beast", "Undead", "Dragon").
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Hit points (total health).
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Hp { get; set; }

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
    /// Collection of attacks the monster can make.
    /// </summary>
    public List<Attack> Attacks { get; set; } = new();
}
