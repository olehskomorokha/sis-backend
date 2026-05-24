using Google.Apis.YouTube.v3.Data;
using SmartInfluence.Collector.YouTube;

namespace SmartInfluence.Collector.Extentions;

public static class FieldsCalculator
{
    public static YouTubeApi.Fields Calculate(Channel channel, YouTubeApi.Interests interests)
    {
        return new YouTubeApi.Fields
        {
            PublishedAt = GetPublishedAtOrNull(channel.Snippet),
            Posts = ToNullableInt(channel.Statistics?.VideoCount),
            SubscriberCount = ToNullableInt(channel.Statistics?.SubscriberCount),
            ViewCount = ToNullableInt(channel.Statistics?.ViewCount),
            Interests = interests
        };
    }

    private static DateTime? GetPublishedAtOrNull(ChannelSnippet? snippet)
    {
        if (snippet is null)
        {
            return null;
        }

        try
        {
            return snippet.PublishedAtDateTimeOffset?.UtcDateTime;
        }
        catch (FormatException)
        {
            return null;
        }
    }

    private static int? ToNullableInt(ulong? value)
    {
        if (value is null)
        {
            return null;
        }

        return value > int.MaxValue ? int.MaxValue : (int)value.Value;
    }
}
