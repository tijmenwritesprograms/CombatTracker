using Microsoft.JSInterop;
using System.Text.Json;

namespace CombatTracker.Web.Services;

/// <summary>
/// Service for interacting with browser localStorage via JSInterop.
/// </summary>
public class LocalStorageService : ILocalStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ILogger<LocalStorageService> _logger;

    public LocalStorageService(IJSRuntime jsRuntime, ILogger<LocalStorageService> logger)
    {
        _jsRuntime = jsRuntime;
        _logger = logger;
    }

    /// <summary>
    /// Saves data to localStorage.
    /// </summary>
    /// <typeparam name="T">Type of data to save</typeparam>
    /// <param name="key">Storage key</param>
    /// <param name="data">Data to save</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> SetItemAsync<T>(string key, T data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving to localStorage with key: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// Retrieves data from localStorage.
    /// </summary>
    /// <typeparam name="T">Type of data to retrieve</typeparam>
    /// <param name="key">Storage key</param>
    /// <returns>Deserialized data or default value if not found</returns>
    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving from localStorage with key: {Key}", key);
            return default;
        }
    }

    /// <summary>
    /// Removes an item from localStorage.
    /// </summary>
    /// <param name="key">Storage key</param>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> RemoveItemAsync(string key)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from localStorage with key: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// Clears all data from localStorage.
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    public async Task<bool> ClearAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.clear");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing localStorage");
            return false;
        }
    }

    /// <summary>
    /// Checks if a key exists in localStorage.
    /// </summary>
    /// <param name="key">Storage key</param>
    /// <returns>True if key exists, false otherwise</returns>
    public async Task<bool> ContainsKeyAsync(string key)
    {
        try
        {
            var value = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
            return value != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking localStorage for key: {Key}", key);
            return false;
        }
    }
}
