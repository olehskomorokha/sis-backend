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
                You analyze a product description and generate tags for finding relevant YouTube channels.

                Return only valid JSON with this exact schema:
                {
                  "channelTags": ["string"],
                  "videoTags": ["string"]
                }

                Rules:
                - Generate ALL tags ONLY in Ukrainian language.
                - channelTags: general influencer/channel topics or niches.
                - videoTags: more specific video-level keywords related to the product.
                - Generate realistic YouTube-related tags.
                - Do not invent unrelated tags.
                - Return 5-15 channelTags.
                - Return 10-25 videoTags.
                - Do not add explanations.
                - Do not wrap JSON in markdown.
                """
            ),
            new UserChatMessage(productDescription)
        };
        var completion = await client.CompleteChatAsync(messages);

        var json = completion.Value.Content[0].Text;

        return JsonSerializer.Deserialize<ProductCriteriaModel>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? new ProductCriteriaModel();
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
