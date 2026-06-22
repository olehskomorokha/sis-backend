using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using SmartInfluence.Services.Exceptions;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class ProductQueryAiService : IProductQueryAiService
{
    private readonly string _apiKey;
    private readonly string _model;
    private readonly IElasticsearchService _elasticsearchService;

    public ProductQueryAiService(IConfiguration configuration, IElasticsearchService elasticsearchService)
    {
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured.");
        _model = configuration["OpenAI:Model"] ?? "gpt-4.1";
        _elasticsearchService = elasticsearchService;
    }

    public async Task<ProductCriteriaModel> ParseProductDescriptionAsync(string productDescription)
    {
        if (string.IsNullOrWhiteSpace(productDescription))
        {
            throw new InvalidProductDescriptionException("Product description is required.");
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
                - Generate ALL tags in Ukrainian language and its copy in english.
                - channelTags: general influencer/channel topics or niches.
                - videoTags: more specific video-level keywords related to the product.
                - Generate realistic YouTube-related tags.
                - Do not invent unrelated tags.
                - Return 5-15 channelTags in ukrainian and english.
                - Return 10-25 videoTags in ukrainian and english.
                - If the user text is random, meaningless, too vague, or not a product/service/campaign description, return:
                  { "channelTags": [], "videoTags": [] }
                - Do not add explanations.
                - Do not wrap JSON in markdown.
                """
            ),
            new UserChatMessage(productDescription)
        };
        var completion = await client.CompleteChatAsync(messages);

        var json = completion.Value.Content[0].Text;    

        var criteria = JsonSerializer.Deserialize<ProductCriteriaModel>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        ) ?? new ProductCriteriaModel();

        if (!HasUsefulTags(criteria))
        {
            throw new InvalidProductDescriptionException(
                "Description must clearly describe a product, service, or advertising campaign.");
        }

        return criteria;
    }

    public async Task<string> AiChannelReviewAsync(string channelId)
    {
        var channel = await _elasticsearchService.GetById(channelId);
        if (channel == null)
        {
            return string.Empty;
        }

        var client = new ChatClient(model: _model, apiKey: _apiKey);

        var channelJson = JsonSerializer.Serialize(BuildReviewInput(channel));

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(
                """
                You analyze YouTube channel data.

                Based on the provided channel JSON, write a short review in Ukrainian.

                Describe:
                - what the channel is about;
                - main content topics;
                - likely target audience;
                - content style;
                - whether the channel may be useful for influencer marketing.

                Rules:
                - Answer only in Ukrainian.
                - Do not invent facts that are not present in the data.
                - If some information is missing, make a careful assumption based on title, description, tags and statistics.
                - Keep the answer concise: 1-2 paragraphs.
                """
            ),
            new UserChatMessage(channelJson)
        };

        var completion = await client.CompleteChatAsync(messages);

        return completion.Value.Content[0].Text;
    }

    private static object BuildReviewInput(object channel)
    {
        var root = JsonNode.Parse(JsonSerializer.Serialize(channel))?.AsObject();
        if (root == null)
        {
            return new { };
        }

        return new
        {
            Name = GetString(root, "name"),
            Description = GetString(root, "description"),
            CountryCode = GetString(root, "countryCode"),
            ChannelTags = GetStringArray(root, "interests", "channelTags"),
            VideoTags = GetStringArray(root, "interests", "videoTags"),
            SubscribersCount = GetInt(root, "fields", "subscriberCount"),
            AvgViews = GetInt(root, "statictics", "perHalfYear", "avgView"),
            AvgLikes = GetInt(root, "statictics", "perHalfYear", "avgLike"),
            AvgComments = GetInt(root, "statictics", "perHalfYear", "avgComment"),
            VideoCount = GetInt(root, "statictics", "perHalfYear", "videoCount")
        };
    }

    private static string? GetString(JsonObject root, params string[] path)
    {
        return GetNode(root, path)?.GetValue<string>();
    }

    private static int? GetInt(JsonObject root, params string[] path)
    {
        var node = GetNode(root, path);
        if (node == null)
        {
            return null;
        }

        if (node is JsonValue value && value.TryGetValue<int>(out var intValue))
        {
            return intValue;
        }

        if (node is JsonValue longValue && longValue.TryGetValue<long>(out var longResult))
        {
            return (int)longResult;
        }

        if (node is JsonValue doubleValue && doubleValue.TryGetValue<double>(out var doubleResult))
        {
            return (int)Math.Round(doubleResult);
        }

        return null;
    }

    private static string[] GetStringArray(JsonObject root, params string[] path)
    {
        return GetNode(root, path) is JsonArray array
            ? array.Select(node => node?.GetValue<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Cast<string>()
                .ToArray()
            : [];
    }

    private static JsonNode? GetNode(JsonObject root, params string[] path)
    {
        JsonNode? current = root;

        foreach (var segment in path)
        {
            current = current?[segment];
            if (current == null)
            {
                return null;
            }
        }

        return current;
    }

    private static bool HasUsefulTags(ProductCriteriaModel criteria)
    {
        return HasTags(criteria.ChannelTags) || HasTags(criteria.VideoTags);
    }

    private static bool HasTags(IEnumerable<string>? tags)
    {
        return tags?.Any(tag => !string.IsNullOrWhiteSpace(tag)) == true;
    }
}
