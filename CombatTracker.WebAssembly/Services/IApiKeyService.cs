namespace CombatTracker.WebAssembly.Services;

/// <summary>
/// Service for managing API keys stored in browser localStorage.
/// </summary>
public interface IApiKeyService
{
    /// <summary>
    /// Initialize the service and load any stored API keys.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Get the OpenAI API key from localStorage.
    /// </summary>
    /// <returns>The API key, or null if not set.</returns>
    Task<string?> GetOpenAIApiKeyAsync();

    /// <summary>
    /// Set the OpenAI API key in localStorage.
    /// </summary>
    /// <param name="apiKey">The API key to store, or null to remove.</param>
    Task SetOpenAIApiKeyAsync(string? apiKey);

    /// <summary>
    /// Check if an OpenAI API key is configured.
    /// </summary>
    /// <returns>True if an API key is stored, false otherwise.</returns>
    Task<bool> HasOpenAIApiKeyAsync();

    /// <summary>
    /// Event raised when API key configuration changes.
    /// </summary>
    event Action? OnChange;
}
