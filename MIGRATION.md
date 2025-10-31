# Migration from Blazor Server to Blazor WebAssembly

This document describes the migration of the CombatTracker application from Blazor Server to Blazor WebAssembly.

## Overview

**Date**: October 2025  
**Migration Type**: Blazor Server → Blazor WebAssembly (Standalone)  
**Reason**: Improved scalability, reduced server load, offline capability, and simpler deployment

## Key Changes

### 1. Project Structure

**Before:**
- `CombatTracker.Web` - Blazor Server application with Interactive Server components

**After:**
- `CombatTracker.WebAssembly` - Blazor WebAssembly standalone application
- `CombatTracker.Web` - Kept for backward reference (can be removed)

### 2. Render Modes

**Before:**
```razor
@rendermode InteractiveServer
```

**After:**
- No `@rendermode` directive needed (WebAssembly components run client-side by default)

### 3. Service Registration

**Before (Program.cs in Server):**
```csharp
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<PartyStateService>();
builder.Services.AddScoped<CombatStateService>();
```

**After (Program.cs in WebAssembly):**
```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<PartyStateService>();
builder.Services.AddScoped<CombatStateService>();
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
```

### 4. State Persistence

**No changes required** - The application already used browser localStorage via JSInterop, which works the same in WebAssembly.

### 5. Error Handling

**Before (Server - Error.razor):**
```csharp
[CascadingParameter]
private HttpContext? HttpContext { get; set; }

protected override void OnInitialized()
{
    requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier;
}
```

**After (WebAssembly - Error.razor):**
```csharp
[SupplyParameterFromQuery]
public string? RequestId { get; set; }

public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
```

Note: `HttpContext` is not available in WebAssembly since there's no server-side execution.

### 6. App Host Configuration

**Before:**
```csharp
builder.AddProject<Projects.CombatTracker_Web>("webfrontend")
```

**After:**
```csharp
builder.AddProject<Projects.CombatTracker_WebAssembly>("webfrontend")
```

### 7. Tests

- Created `CombatTracker.WebAssembly.Tests` with all 108 tests migrated
- Tests use bUnit and xUnit (same as before)
- All tests pass without modification (services work the same client-side)

## Benefits of Migration

1. **Scalability**: No server-side resources needed per user
2. **Performance**: UI rendering happens locally in the browser
3. **Offline Support**: Can add service workers for offline functionality
4. **Deployment**: Can host on any static file server (GitHub Pages, Azure Static Web Apps, etc.)
5. **Cost**: Reduced hosting costs (no active server runtime needed)
6. **Latency**: No network round-trips for UI updates

## Considerations

1. **Initial Load Time**: Larger initial download (~2-3MB for Blazor WASM runtime)
2. **Browser Requirements**: Requires WebAssembly-capable browser (all modern browsers)
3. **No Server-Side Code**: Cannot access server-only APIs like HttpContext
4. **AOT Compilation**: Can enable AOT for better runtime performance (future enhancement)

## Testing the Migration

### Build and Test
```bash
# Build the solution
dotnet build

# Run all tests (216 total)
dotnet test

# Run WebAssembly app
cd CombatTracker.WebAssembly
dotnet run
```

### Verify Functionality
1. ✅ Party Management - Create, edit, delete parties and characters
2. ✅ Combat Setup - Add monsters, roll initiative
3. ✅ Combat Tracking - Manage turn order, HP, damage/healing
4. ✅ Data Management - Export/import, localStorage persistence
5. ✅ All 108 tests passing

## Rollback Plan

If needed, the original Blazor Server project (`CombatTracker.Web`) is preserved and can be restored by:
1. Reverting AppHost project reference to `CombatTracker.Web`
2. Updating Program.cs in AppHost

## Future Enhancements

1. **Progressive Web App (PWA)**: Add service worker for offline support
2. **AOT Compilation**: Enable ahead-of-time compilation for faster startup
3. **Lazy Loading**: Implement assembly lazy loading for smaller initial download
4. **Static Hosting**: Deploy to CDN for global distribution

## References

- [Blazor WebAssembly Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models#blazor-webassembly)
- [Blazor Server vs WebAssembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models)
- [.NET Aspire with Blazor](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/build-aspire-apps-with-blazor)
