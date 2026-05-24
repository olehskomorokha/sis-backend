namespace SmartInfluence.Collector.Extentions;

public class StringConverter
{
    public static string ConvertChannelIdToPlaylistId(string channelId)
    {
        if (string.IsNullOrWhiteSpace(channelId))
            throw new ArgumentException("Channel ID cannot be empty.");

        if (!channelId.StartsWith("UC"))
            throw new ArgumentException("Invalid YouTube Channel ID.");

        return "UU" + channelId.Substring(2);
    }
}