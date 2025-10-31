using Microsoft.JSInterop;

namespace CombatTracker.WebAssembly.Services;

/// <summary>
/// Service for handling keyboard shortcuts throughout the application
/// </summary>
public class KeyboardShortcutService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly Dictionary<string, Func<Task>> _shortcuts = new();
    private IJSObjectReference? _module;
    private DotNetObjectReference<KeyboardShortcutService>? _dotNetRef;
    private bool _initialized;

    public KeyboardShortcutService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Initialize the keyboard shortcut service
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_initialized) return;

        try
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/keyboard-shortcuts.js");
            _dotNetRef = DotNetObjectReference.Create(this);
            await _module.InvokeVoidAsync("initialize", _dotNetRef);
            _initialized = true;
        }
        catch (JSException)
        {
            // Keyboard shortcuts are optional features - JS module loading may fail in some environments
            // (e.g., during pre-rendering or in certain hosting scenarios). Silently fail to avoid breaking the app.
        }
        catch (InvalidOperationException)
        {
            // JSRuntime may not be available during pre-rendering
        }
    }

    /// <summary>
    /// Register a keyboard shortcut
    /// </summary>
    /// <param name="key">The key combination (e.g., "n", "ctrl+s", "shift+d")</param>
    /// <param name="action">The action to execute when the shortcut is triggered</param>
    public void RegisterShortcut(string key, Func<Task> action)
    {
        _shortcuts[key.ToLowerInvariant()] = action;
    }

    /// <summary>
    /// Unregister a keyboard shortcut
    /// </summary>
    public void UnregisterShortcut(string key)
    {
        _shortcuts.Remove(key.ToLowerInvariant());
    }

    /// <summary>
    /// Called from JavaScript when a keyboard shortcut is triggered
    /// </summary>
    [JSInvokable]
    public async Task HandleShortcut(string key)
    {
        if (_shortcuts.TryGetValue(key.ToLowerInvariant(), out var action))
        {
            await action();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            try
            {
                await _module.InvokeVoidAsync("dispose");
                await _module.DisposeAsync();
            }
            catch
            {
                // Ignore disposal errors
            }
        }

        _dotNetRef?.Dispose();
    }
}
