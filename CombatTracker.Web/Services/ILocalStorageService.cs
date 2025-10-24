namespace CombatTracker.Web.Services;

/// <summary>
/// Interface for local storage operations.
/// </summary>
public interface ILocalStorageService
{
    /// <summary>
    /// Saves data to localStorage.
    /// </summary>
    Task<bool> SetItemAsync<T>(string key, T data);

    /// <summary>
    /// Retrieves data from localStorage.
    /// </summary>
    Task<T?> GetItemAsync<T>(string key);

    /// <summary>
    /// Removes an item from localStorage.
    /// </summary>
    Task<bool> RemoveItemAsync(string key);

    /// <summary>
    /// Clears all data from localStorage.
    /// </summary>
    Task<bool> ClearAsync();

    /// <summary>
    /// Checks if a key exists in localStorage.
    /// </summary>
    Task<bool> ContainsKeyAsync(string key);
}
