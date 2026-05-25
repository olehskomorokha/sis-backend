using Google.Apis.YouTube.v3.Data;
using SmartInfluence.Collector.YouTube;
using System.Text.RegularExpressions;

namespace SmartInfluence.Collector.Extentions;

public static class InterestsCalculator
{
    private static readonly Regex TagRegex = new("\"([^\"]+)\"|[^,\\s]+", RegexOptions.Compiled);

    public static YouTubeApi.Interests Calculate(
        Channel channel,
        IReadOnlyCollection<YouTubeApi.VideoDetailModel> videos)
    {
        return new YouTubeApi.Interests
        {
            ChannelTags = ExtractChannelTags(channel),
            VideoTags = ExtractVideoTags(videos)
        };
    }

    private static string[] ExtractChannelTags(Channel channel)
    {
        var keywordTags = SplitTags(channel.BrandingSettings?.Channel?.Keywords);
        var topicTags = SplitTags(string.Join(",", channel.TopicDetails?.TopicCategories ?? []));

        return keywordTags
            .Concat(topicTags)
            .Select(NormalizeTopic)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string[] ExtractVideoTags(IReadOnlyCollection<YouTubeApi.VideoDetailModel> videos)
    {
        return videos
            .SelectMany(video => SplitTags(video.Tags)
                .Concat(SplitTags(video.TopicCategories)))
            .Select(NormalizeTopic)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IEnumerable<string> SplitTags(string? tags)
    {
        if (string.IsNullOrWhiteSpace(tags))
        {
            return [];
        }

        return TagRegex
            .Matches(tags)
            .Select(match => match.Groups[1].Success ? match.Groups[1].Value : match.Value)
            .Select(tag => tag.Trim())
            .Where(tag => !string.IsNullOrWhiteSpace(tag));
    }

    private static string? NormalizeTopic(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            return value.Trim();
        }

        var segment = uri.Segments.LastOrDefault()?.Trim('/');
        return string.IsNullOrWhiteSpace(segment) ? value.Trim() : Uri.UnescapeDataString(segment);
    }
}
