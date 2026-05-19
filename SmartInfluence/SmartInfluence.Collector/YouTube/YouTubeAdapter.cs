using System.Text;
using System.Text.Json;
using SmartInfluence.Data.Entities;

namespace SmartInfluence.Collector.YouTube;

public sealed class YouTubeAdapter
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyList<Influencers>> ReadInfluencersFromJsonAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await using var stream = File.OpenRead(path);
        var bloggers = await JsonSerializer.DeserializeAsync<List<YouTubeApi.UkrainianYouTubeBloggerDto>>(stream, JsonSerializerOptions, cancellationToken);

        if (bloggers is null || bloggers.Count == 0)
        {
            return [];
        }

        return bloggers.Select(MapToInfluencer).ToArray();
    }

    public Influencers MapToInfluencer(YouTubeApi.UkrainianYouTubeBloggerDto blogger)
    {
        ArgumentNullException.ThrowIfNull(blogger);

        var title = DecodeEscapedJsonString(blogger.Title);
        var description = DecodeEscapedJsonString(blogger.Description);
        var country = DecodeEscapedJsonString(blogger.Country);
        var customUrl = DecodeEscapedJsonString(blogger.CustomUrl);

        return new Influencers
        {
            InfluencerId = blogger.ChannelId,
            UserName = BuildUserName(customUrl, blogger.ChannelId),
            FullName = title,
            Platform = "YouTube",
            Bio = description,
            Country = country,
            Lenguage = InferLanguage(title, description, country),
            FollowersCount = ConvertToInt32(blogger.SubscriberCount),
            FollowingCount = 0,
            PostsCount = ConvertToInt32(blogger.VideoCount),
            AvgViews = CalculateAverageViews(blogger.ViewCount, blogger.VideoCount),
            AvgLikes = 0,
            AvgComments = 0
        };
    }

    private static string BuildUserName(string customUrl, string channelId)
    {
        if (!string.IsNullOrWhiteSpace(customUrl))
        {
            return customUrl.TrimStart('@');
        }

        return channelId;
    }

    private static string DecodeEscapedJsonString(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if (!value.Contains('\\'))
        {
            return value;
        }

        var wrappedJsonString = "\"" + EscapeForJsonStringLiteral(value) + "\"";
        return JsonSerializer.Deserialize<string>(wrappedJsonString) ?? value;
    }

    private static string EscapeForJsonStringLiteral(string value)
    {
        var buffer = new StringBuilder(value.Length + 8);

        foreach (var character in value)
        {
            switch (character)
            {
                case '"':
                    buffer.Append("\\\"");
                    break;
                case '\r':
                    buffer.Append("\\r");
                    break;
                case '\n':
                    buffer.Append("\\n");
                    break;
                case '\t':
                    buffer.Append("\\t");
                    break;
                case '\b':
                    buffer.Append("\\b");
                    break;
                case '\f':
                    buffer.Append("\\f");
                    break;
                default:
                    if (char.IsControl(character))
                    {
                        buffer.Append($"\\u{(int)character:x4}");
                    }
                    else
                    {
                        buffer.Append(character);
                    }

                    break;
            }
        }

        return buffer.ToString();
    }

    private static int ConvertToInt32(ulong? value)
    {
        if (!value.HasValue)
        {
            return 0;
        }

        return value.Value > int.MaxValue ? int.MaxValue : (int)value.Value;
    }

    private static decimal CalculateAverageViews(ulong? totalViews, ulong? videoCount)
    {
        if (!totalViews.HasValue || !videoCount.HasValue || videoCount.Value == 0)
        {
            return 0;
        }

        return decimal.Divide(totalViews.Value, videoCount.Value);
    }

    private static string InferLanguage(string title, string description, string country)
    {
        var text = string.Join(' ', new[] { title, description, country }.Where(x => !string.IsNullOrWhiteSpace(x)));

        return ContainsCyrillic(text) ? "uk" : "en";
    }

    private static bool ContainsCyrillic(string value)
    {
        foreach (var character in value)
        {
            if (character is >= '\u0400' and <= '\u04FF')
            {
                return true;
            }
        }

        return false;
    }
}
