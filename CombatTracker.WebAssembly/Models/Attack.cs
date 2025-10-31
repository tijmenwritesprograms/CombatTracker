using System.ComponentModel.DataAnnotations;

namespace CombatTracker.WebAssembly.Models;

/// <summary>
/// Represents an attack that a monster can make.
/// </summary>
public class Attack
{
    /// <summary>
    /// Name of the attack (e.g., "Longsword", "Bite", "Fire Bolt").
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Attack bonus to hit.
    /// </summary>
    [Range(-5, 20)]
    public int AttackBonus { get; set; }

    /// <summary>
    /// Damage description (e.g., "2d6+4 slashing").
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Damage { get; set; } = string.Empty;
}
