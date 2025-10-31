namespace CombatTracker.WebAssembly;

public class WeatherApiClient(HttpClient httpClient)
{
    // Simplified for WebAssembly - can be extended with proper API calls later
    public async Task<WeatherForecast[]?> GetWeatherAsync(int maxItems = 10, CancellationToken cancellationToken = default)
    {
        // For now, return null - this would need to be implemented with actual API endpoint
        await Task.CompletedTask;
        return null;
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
