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
                - Generate ALL tags in Ukrainian language and its copy in english.
                - channelTags: general influencer/channel topics or niches.
                - videoTags: more specific video-level keywords related to the product.
                - Generate realistic YouTube-related tags.
                - Do not invent unrelated tags.
                - Return 5-15 channelTags in ukrainian and english.
                - Return 10-25 videoTags in ukrainian and english.
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

    public async Task<string> AiChannelReviewAsync(string channelId)
    {
        var channel = await _elasticsearchService.GetById(channelId);
        if (channel == null)
        {
            return string.Empty;
        }

        var client = new ChatClient(model: _model, apiKey: _apiKey);

        var channelJson = JsonSerializer.Serialize(channel, new JsonSerializerOptions
        {
            WriteIndented = true
        });

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
}
