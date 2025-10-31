using System.ComponentModel.DataAnnotations;

namespace CombatTracker.WebAssembly.Models;

/// <summary>
/// Represents a group of player characters.
/// </summary>
public class Party
{
    /// <summary>
    /// Unique identifier for the party.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the party or campaign.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Collection of characters in this party.
    /// </summary>
    public List<Character> Characters { get; set; } = new();
}
