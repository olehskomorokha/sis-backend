using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Exceptions;

public static class ScoreCalculator
{
    private const decimal BrandFitWeight = 0.32m;
    private const decimal EngagementWeight = 0.48m;
    private const decimal PostFrequencyWeight = 0.2m;
    private const float DefaultMaxExpectedEngagementRate = 10f;
    private const float DefaultMaxExpectedPostsPerDay = 1f;

    public static decimal CalculateBrandFitScore(
        double? elasticScore,
        double minElasticScore,
        double maxElasticScore)
    {
        if (!elasticScore.HasValue || maxElasticScore <= 0)
        {
            return 0m;
        }

        var normalizedScore = NormalizeMinMax(elasticScore.Value, minElasticScore, maxElasticScore);
        return ToHundredScale(normalizedScore);
    }

    public static decimal CalculateEngagementScore(
        float engagementRate,
        float maxExpectedEngagementRate = DefaultMaxExpectedEngagementRate)
    {
        if (maxExpectedEngagementRate <= 0)
        {
            return 0m;
        }

        var normalizedScore = Math.Clamp(engagementRate / maxExpectedEngagementRate, 0f, 1f);
        return ToHundredScale((decimal)normalizedScore);
    }
    
    public static decimal CalculatePostFrequencyScore(
        float postsPerDay,
        float maxExpectedPostsPerDay = DefaultMaxExpectedPostsPerDay)
    {
        if (maxExpectedPostsPerDay <= 0)
        {
            return 0m;
        }

        var normalizedScore = Math.Clamp(postsPerDay / maxExpectedPostsPerDay, 0f, 1f);
        return ToHundredScale((decimal)normalizedScore);
    }

    public static int CalculateTotalScore(
        decimal brandFitScore,
        decimal engagementScore,
        decimal postFrequencyScore)
    {
        var totalScore = brandFitScore * BrandFitWeight
            + engagementScore * EngagementWeight
            + postFrequencyScore * PostFrequencyWeight;
        return (int)Math.Round(totalScore, MidpointRounding.AwayFromZero);
    }

    public static decimal CalculatePredictedEngagement(float engagementRate, int viewCount)
    {
        if (viewCount <= 0 || engagementRate <= 0)
        {
            return 0m;
        }

        var predictedEngagement = ((decimal)engagementRate / 100m) * viewCount;
        return Math.Round(predictedEngagement, 2, MidpointRounding.AwayFromZero);
    }

    public static void ApplyCalculatedScores(List<RecommendedChannelModel> channels)
    {
        if (channels.Count == 0)
        {
            return;
        }

        var elasticScores = channels
            .Select(channel => channel.Score ?? 0d)
            .ToList();

        var minElasticScore = elasticScores.Min();
        var maxElasticScore = elasticScores.Max();

        foreach (var channel in channels)
        {
            channel.BrandFitScore = CalculateBrandFitScore(
                channel.Score,
                minElasticScore,
                maxElasticScore);
            channel.EngagementScore = CalculateEngagementScore(channel.EngagementRate);
            channel.PostFrequencyScore = CalculatePostFrequencyScore(channel.PostPerDay);
            channel.PredictedEngagement = CalculatePredictedEngagement(channel.EngagementRate, channel.AvgView);
            channel.TotalScore = CalculateTotalScore(
                channel.BrandFitScore,
                channel.EngagementScore,
                channel.PostFrequencyScore);
        }
    }

    private static decimal NormalizeMinMax(double value, double min, double max)
    {
        if (max <= min)
        {
            return value > 0 ? 1m : 0m;
        }

        var normalized = (value - min) / (max - min);
        return Math.Clamp((decimal)normalized, 0m, 1m);
    }

    private static decimal ToHundredScale(decimal normalizedValue)
    {
        return Math.Round(normalizedValue * 100m, 2, MidpointRounding.AwayFromZero);
    }
}
