using System.Text.Json;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class ProductQueryAiService : IProductQueryAiService
{
    private readonly string _apiKey;
    private readonly string _model;

    public ProductQueryAiService(IConfiguration configuration)
    {
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured.");
        _model = configuration["OpenAI:Model"] ?? "gpt-4.1";
    }

    public async Task<ProductCriteriaModel> ParseProductDescriptionAsync(string productDescription)
    {
        if (string.IsNullOrWhiteSpace(productDescription))
        {
            return new ProductCriteriaModel();
        }

        var client = new ChatClient(model: _model, apiKey: _apiKey);

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(
                """
                Extract influencer marketing targeting criteria from the user text.
                Return only valid JSON with this exact schema:
                {
                  "productCategory": "string",
                  "topics": ["string"],
                  "platforms": ["instagram|tiktok|youtube|telegram|x|facebook"],
                  "targetCountry": "string",
                  "targetGender": "string",
                  "ageMin": 0,
                  "ageMax": 0,
                  "maxFakeFollowersPercent": 0
                }
                Use empty strings, empty arrays, or nulls when the field is unknown.
                Do not use 0 for unknown numeric fields.
                """
            ),
            new UserChatMessage(productDescription)
        };

        var completion = await client.CompleteChatAsync(messages);
        var content = completion.Value.Content.FirstOrDefault()?.Text;
        if (string.IsNullOrWhiteSpace(content))
        {
            return new ProductCriteriaModel();
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            return new ProductCriteriaModel
            {
                ProductCategory = GetString(root, "productCategory"),
                Topics = GetStringArray(root, "topics"),
                Platforms = GetStringArray(root, "platforms"),
                TargetCountry = GetString(root, "targetCountry"),
                TargetGender = GetString(root, "targetGender"),
                AgeMin = GetInt32Nullable(root, "ageMin"),
                AgeMax = GetInt32Nullable(root, "ageMax"),
                MaxFakeFollowersPercent = GetDecimalNullable(root, "maxFakeFollowersPercent")
            };
        }
        catch (JsonException)
        {
            return new ProductCriteriaModel
            {
                ProductCategory = productDescription.Trim()
            };
        }
    }

    private static string GetString(JsonElement root, string propertyName)
    {
        return root.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? string.Empty
            : string.Empty;
    }

    private static List<string> GetStringArray(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var value) || value.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return value.EnumerateArray()
            .Where(x => x.ValueKind == JsonValueKind.String)
            .Select(x => x.GetString())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .ToList();
    }

    private static int? GetInt32Nullable(JsonElement root, string propertyName)
    {
        return root.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var result)
            ? result
            : null;
    }

    private static decimal? GetDecimalNullable(JsonElement root, string propertyName)
    {
        return root.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var result)
            ? result
            : null;
    }
}
