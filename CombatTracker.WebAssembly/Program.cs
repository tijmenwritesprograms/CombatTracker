using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using CombatTracker.WebAssembly;
using CombatTracker.WebAssembly.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add HTTP client for API calls
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

// Add party state management service
builder.Services.AddScoped<PartyStateService>();

// Add combat state management service
builder.Services.AddScoped<CombatStateService>();

// Add local storage service for browser localStorage access
builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();

// Add storage state service for coordinating persistence
builder.Services.AddScoped<StorageStateService>();

// Add keyboard shortcut service for keyboard navigation
builder.Services.AddScoped<KeyboardShortcutService>();

// Add API key service for managing OpenAI API keys
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

// Add statblock parser service for AI-powered monster parsing
builder.Services.AddScoped<IStatblockParserService, OpenAIStatblockParserService>();

// Add WeatherApiClient (if needed for API service integration)
builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    // Configure base address for API service
    // In WASM, this will need to be configured based on environment
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress);
});

await builder.Build().RunAsync();
