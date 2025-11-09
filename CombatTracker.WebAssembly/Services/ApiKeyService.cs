namespace CombatTracker.WebAssembly.Services;

/// <summary>
/// Service for managing API keys stored in browser localStorage.
/// </summary>
public class ApiKeyService : IApiKeyService
{
    private const string OpenAIApiKeyStorageKey = "openai_api_key";
    
    private readonly ILocalStorageService _localStorage;
    private string? _cachedOpenAIApiKey;

    public event Action? OnChange;

    public ApiKeyService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>
    /// Initialize the service and load any stored API keys.
    /// </summary>
    public async Task InitializeAsync()
    {
        _cachedOpenAIApiKey = await _localStorage.GetItemAsync<string>(OpenAIApiKeyStorageKey);
    }

    /// <summary>
    /// Get the OpenAI API key from localStorage.
    /// </summary>
    /// <returns>The API key, or null if not set.</returns>
    public async Task<string?> GetOpenAIApiKeyAsync()
    {
        if (_cachedOpenAIApiKey == null)
        {
            _cachedOpenAIApiKey = await _localStorage.GetItemAsync<string>(OpenAIApiKeyStorageKey);
        }
        return _cachedOpenAIApiKey;
    }

    /// <summary>
    /// Set the OpenAI API key in localStorage.
    /// </summary>
    /// <param name="apiKey">The API key to store, or null to remove.</param>
    public async Task SetOpenAIApiKeyAsync(string? apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            await _localStorage.RemoveItemAsync(OpenAIApiKeyStorageKey);
            _cachedOpenAIApiKey = null;
        }
        else
        {
            await _localStorage.SetItemAsync(OpenAIApiKeyStorageKey, apiKey);
            _cachedOpenAIApiKey = apiKey;
        }
        OnChange?.Invoke();
    }

    /// <summary>
    /// Check if an OpenAI API key is configured.
    /// </summary>
    /// <returns>True if an API key is stored, false otherwise.</returns>
    public async Task<bool> HasOpenAIApiKeyAsync()
    {
        var key = await GetOpenAIApiKeyAsync();
        return !string.IsNullOrWhiteSpace(key);
    }
}
