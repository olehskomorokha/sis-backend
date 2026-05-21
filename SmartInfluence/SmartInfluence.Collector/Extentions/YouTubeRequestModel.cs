namespace SmartInfluence.Collector.Extentions;

public class YouTubeRequestModel
{
    public Google.Apis.YouTube.v3.YouTubeService Service { get; set; }

    public int Count { get; set; }

    public CancellationToken CancellationToken { get; set; }
}