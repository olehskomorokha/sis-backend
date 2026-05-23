using SmartInfluence.Collector.YouTube;

namespace SmartInfluence.Collector.Extentions;

public class StatisticsCalculator
{
    public static YouTubeApi.PerHalfYear CalculatePerHalfYear(
        IReadOnlyCollection<YouTubeApi.VideoDetailModel> videos)
    {
        var videoCount = videos.Count;

        if (videoCount == 0)
        {
            return new YouTubeApi.PerHalfYear
            {
                VideoCount = 0,
                PostPerDay = 0,
                AvgView = 0,
                AvgLike = 0,
                AvgComment = 0
            };
        }

        return new YouTubeApi.PerHalfYear
        {
            VideoCount = videoCount,
            PostPerDay = (int)Math.Round(videoCount / 182.5),
            AvgView = (int)videos.Average(x => (double)(x.ViewCount ?? 0)),
            AvgLike = (int)videos.Average(x => (double)(x.LikeCount ?? 0)),
            AvgComment = (int)videos.Average(x => (double)(x.CommentCount ?? 0))
        };
    }
}