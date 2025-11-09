using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CombatTracker.WebAssembly.Models;

namespace CombatTracker.WebAssembly.Services;

/// <summary>
/// OpenAI-based implementation of statblock parser service.
/// </summary>
public class OpenAIStatblockParserService : IStatblockParserService
{
    private readonly HttpClient _httpClient;
    private readonly IApiKeyService _apiKeyService;
    private const string OpenAIApiUrl = "https://api.openai.com/v1/chat/completions";
    private const string DefaultModel = "gpt-4o-mini";
    private const int MaxRetries = 2;
    private const int TimeoutSeconds = 30;

    public OpenAIStatblockParserService(HttpClient httpClient, IApiKeyService apiKeyService)
    {
        _httpClient = httpClient;
        _apiKeyService = apiKeyService;
        _httpClient.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
    }

    /// <summary>
    /// Check if the service is properly configured with an API key.
    /// </summary>
    public async Task<bool> IsConfiguredAsync()
    {
        return await _apiKeyService.HasOpenAIApiKeyAsync();
    }

    /// <summary>
    /// Parse a monster statblock text and extract structured data.
    /// </summary>
    public async Task<Monster?> ParseStatblockAsync(string statblockText)
    {
        if (string.IsNullOrWhiteSpace(statblockText))
        {
            return null;
        }

        var apiKey = await _apiKeyService.GetOpenAIApiKeyAsync();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured. Please configure it in Settings.");
        }

        // Retry logic for transient failures
        Exception? lastException = null;
        for (int attempt = 0; attempt <= MaxRetries; attempt++)
        {
            try
            {
                return await ParseWithOpenAIAsync(statblockText, apiKey);
            }
            catch (HttpRequestException ex)
            {
                lastException = ex;
                if (attempt < MaxRetries)
                {
                    // Exponential backoff: 1s, 2s
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                }
            }
            catch (TaskCanceledException ex)
            {
                lastException = ex;
                // Don't retry timeouts
                break;
            }
        }

        throw new InvalidOperationException(
            $"Failed to parse statblock after {MaxRetries + 1} attempts. {lastException?.Message}",
            lastException);
    }

    private async Task<Monster?> ParseWithOpenAIAsync(string statblockText, string apiKey)
    {
        var systemPrompt = BuildSystemPrompt();
        var userPrompt = BuildUserPrompt(statblockText);

        var request = new OpenAIRequest
        {
            Model = DefaultModel,
            Messages = new[]
            {
                new OpenAIMessage { Role = "system", Content = systemPrompt },
                new OpenAIMessage { Role = "user", Content = userPrompt }
            },
            Temperature = 0.1,
            ResponseFormat = new { type = "json_object" }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, OpenAIApiUrl)
        {
            Headers = { { "Authorization", $"Bearer {apiKey}" } },
            Content = JsonContent.Create(request, options: new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            })
        };

        var response = await _httpClient.SendAsync(httpRequest);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"OpenAI API request failed with status {response.StatusCode}. Response: {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var openAIResponse = JsonSerializer.Deserialize<OpenAIResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (openAIResponse?.Choices == null || openAIResponse.Choices.Length == 0)
        {
            throw new InvalidOperationException("OpenAI returned an empty response.");
        }

        var jsonContent = openAIResponse.Choices[0].Message?.Content;
        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            throw new InvalidOperationException("OpenAI returned no content.");
        }

        // Parse the JSON response into a Monster object
        var monster = JsonSerializer.Deserialize<Monster>(jsonContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return monster;
    }

    private string BuildSystemPrompt()
    {
        return @"You are a D&D 5e statblock parser. Extract monster data from text and return it as valid JSON matching this exact structure:

{
  ""name"": ""Monster Name"",
  ""size"": ""Medium"",
  ""type"": ""Humanoid"",
  ""subtype"": ""Orc"",
  ""alignment"": ""Chaotic Evil"",
  ""ac"": 13,
  ""armorType"": ""hide armor"",
  ""hp"": 15,
  ""hpFormula"": ""2d8 + 6"",
  ""speed"": 30,
  ""flySpeed"": 0,
  ""swimSpeed"": 0,
  ""climbSpeed"": 0,
  ""burrowSpeed"": 0,
  ""abilities"": {
    ""strength"": 16,
    ""dexterity"": 12,
    ""constitution"": 16,
    ""intelligence"": 7,
    ""wisdom"": 11,
    ""charisma"": 10
  },
  ""savingThrows"": ""Str +5, Dex +3"",
  ""skills"": ""Intimidation +2"",
  ""vulnerabilities"": null,
  ""resistances"": null,
  ""immunities"": null,
  ""conditionImmunities"": null,
  ""senses"": ""Darkvision 60 ft., Passive Perception 10"",
  ""languages"": ""Common, Orc"",
  ""challengeRating"": ""1/2"",
  ""experiencePoints"": 100,
  ""proficiencyBonus"": 2,
  ""initiativeModifier"": 1,
  ""traits"": [
    {
      ""name"": ""Aggressive"",
      ""description"": ""As a bonus action, the orc can move up to its speed toward a hostile creature that it can see.""
    }
  ],
  ""actions"": [
    {
      ""name"": ""Greataxe"",
      ""description"": ""Melee Weapon Attack: +5 to hit, reach 5 ft., one target. Hit: 9 (1d12 + 3) slashing damage."",
      ""attackType"": ""Melee Weapon Attack"",
      ""attackBonus"": 5,
      ""reach"": 5,
      ""damageFormula"": ""1d12 + 3"",
      ""damageType"": ""slashing""
    }
  ],
  ""bonusActions"": [],
  ""reactions"": [],
  ""legendaryActions"": [],
  ""legendaryActionsPerRound"": 0,
  ""lairActions"": []
}

Rules:
- Extract all fields accurately from the statblock text
- Use null for missing optional fields
- Calculate initiativeModifier from Dexterity modifier: (dexterity - 10) / 2
- Parse ability scores carefully (STR, DEX, CON, INT, WIS, CHA)
- Extract traits, actions, bonus actions, reactions, and legendary actions separately
- For actions, extract attack details: attackType, attackBonus, reach/range, damageFormula, damageType
- Return ONLY valid JSON, no additional text or explanation";
    }

    private string BuildUserPrompt(string statblockText)
    {
        return $"Parse this D&D 5e monster statblock and return the structured JSON:\n\n{statblockText}";
    }

    #region OpenAI API Models

    private class OpenAIRequest
    {
        public string Model { get; set; } = DefaultModel;
        public OpenAIMessage[] Messages { get; set; } = Array.Empty<OpenAIMessage>();
        public double Temperature { get; set; } = 0.1;
        public object? ResponseFormat { get; set; }
    }

    private class OpenAIMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    private class OpenAIResponse
    {
        public string? Id { get; set; }
        public string? Object { get; set; }
        public long Created { get; set; }
        public string? Model { get; set; }
        public OpenAIChoice[]? Choices { get; set; }
    }

    private class OpenAIChoice
    {
        public int Index { get; set; }
        public OpenAIMessage? Message { get; set; }
        public string? FinishReason { get; set; }
    }

    #endregion
}
