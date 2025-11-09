using CombatTracker.WebAssembly.Models;

namespace CombatTracker.WebAssembly.Services;

/// <summary>
/// Service for parsing D&D 5e monster statblocks using AI.
/// </summary>
public interface IStatblockParserService
{
    /// <summary>
    /// Parse a monster statblock text and extract structured data.
    /// </summary>
    /// <param name="statblockText">The raw statblock text to parse.</param>
    /// <returns>A parsed Monster object, or null if parsing fails.</returns>
    Task<Monster?> ParseStatblockAsync(string statblockText);

    /// <summary>
    /// Check if the service is properly configured and ready to use.
    /// </summary>
    /// <returns>True if the service can be used, false otherwise.</returns>
    Task<bool> IsConfiguredAsync();
}

/// <summary>
/// Result of a statblock parsing operation.
/// </summary>
public class StatblockParseResult
{
    /// <summary>
    /// Indicates whether the parsing was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The parsed monster data, if successful.
    /// </summary>
    public Monster? Monster { get; set; }

    /// <summary>
    /// Error message if parsing failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Create a successful result.
    /// </summary>
    public static StatblockParseResult CreateSuccess(Monster monster)
    {
        return new StatblockParseResult
        {
            Success = true,
            Monster = monster
        };
    }

    /// <summary>
    /// Create a failed result.
    /// </summary>
    public static StatblockParseResult CreateFailure(string errorMessage)
    {
        return new StatblockParseResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
