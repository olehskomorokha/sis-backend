using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Mappers;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class ElasticsearchService : IElasticsearchService
{
    private const string YoutuberIndex = "youtube";

    private readonly ElasticsearchClient _client;
    
    public ElasticsearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<List<string>> GetAllBloggerTags()
    {
        var response = await _client.SearchAsync<object>(s => s
            .Index(YoutuberIndex)
            .Size(0)
            .Aggregations(aggs => aggs
                .Add("unique_channel_tags", a => a
                    .Terms(t => t
                        .Field("interests.channelTags.keyword")
                        .Size(1000)
                    )
                )
                .Add("unique_video_tags", a => a
                    .Terms(t => t
                        .Field("interests.videoTags.keyword")
                        .Size(1000)
                    )
                )
            )
        );

        return new[] { "unique_channel_tags", "unique_video_tags" }
            .SelectMany(aggregationName => response.Aggregations?
                .GetStringTerms(aggregationName)?
                .Buckets
                .Select(bucket => bucket.Key.ToString()) ?? [])
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .SelectMany(key => key!.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(IsUkrainianTag)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool IsUkrainianTag(string tag)
    {
        return tag.Contains("україн", StringComparison.OrdinalIgnoreCase)
               || tag.Any(letter => "іїєґІЇЄҐ".Contains(letter));
    }

    public async Task<List<RecommendedChannelModel>> RecommendBloggersAsync(
        ProductCriteriaModel criteria,
        InfluencerRecommendationFiltersModel filters)
    {
        var size = Math.Clamp(filters.ResultCount, 1, 500);
        var channelTags = NormalizeTags(criteria.ChannelTags)
            .Concat(NormalizeTags(filters.Tags))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var videoTags = NormalizeTags(criteria.VideoTags)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var filterQueries = new List<Action<QueryDescriptor<RecommendedChannelDocument>>>();

        if (!string.IsNullOrWhiteSpace(filters.Country))
        {
            filterQueries.Add(q => q.Term(t => t
                .Field("countryCode.keyword")
                .Value(filters.Country)
                .CaseInsensitive(true)));
        }

        if (filters.MinFollowersCount.HasValue)
        {
            filterQueries.Add(q => q.Range(r => r
                .NumberRange(nr => nr
                    .Field("fields.subscriberCount")
                    .Gte(filters.MinFollowersCount.Value))));
        }

        if (filters.MinAvgViews.HasValue)
        {
            filterQueries.Add(q => q.Range(r => r
                .NumberRange(nr => nr
                    .Field("statictics.perHalfYear.avgView")
                    .Gte(filters.MinAvgViews.Value))));
        }

        var shouldQueries = new List<Action<QueryDescriptor<RecommendedChannelDocument>>>();

        foreach (var tag in channelTags)
        {
            shouldQueries.Add(q => q.Wildcard(w => w
                .Field("interests.channelTags")
                .Value($"*{tag}*")
                .CaseInsensitive(true)
                .Boost(4)));
            shouldQueries.Add(q => q.Wildcard(w => w
                .Field("name")
                .Value($"*{tag}*")
                .CaseInsensitive(true)
                .Boost(2)));
            shouldQueries.Add(q => q.Wildcard(w => w
                .Field("description")
                .Value($"*{tag}*")
                .CaseInsensitive(true)
                .Boost(1.5f)));
        }

        foreach (var tag in videoTags)
        {
            shouldQueries.Add(q => q.Wildcard(w => w
                .Field("interests.videoTags")
                .Value($"*{tag}*")
                .CaseInsensitive(true)
                .Boost(3)));
            shouldQueries.Add(q => q.Wildcard(w => w
                .Field("interests.videoTitle")
                .Value($"*{tag}*")
                .CaseInsensitive(true)
                .Boost(2)));
        }

        var response = await _client.SearchAsync<RecommendedChannelDocument>(s =>
        {
            s.Index(YoutuberIndex).Size(size);

            if (shouldQueries.Count == 0 && filterQueries.Count == 0)
            {
                s.Query(q => q.MatchAll(_ => { }));
                return;
            }

            s.Query(q => q.Bool(b =>
            {
                if (filterQueries.Count > 0)
                {
                    b.Filter(filterQueries.ToArray());
                }

                if (shouldQueries.Count > 0)
                {
                    b.Should(shouldQueries.ToArray())
                        .MinimumShouldMatch(1);
                }
            }));
        });

        if (!response.IsValidResponse)
        {
            return [];
        }

        return response.Documents
            .Select(MapToRecommendedChannelModel)
            .ToList();
    }

    public async Task<object> GetById(string id)
    {
        var response = await _client.SearchAsync<object>(s => s
            .Index("youtube")
            .Size(1)
            .Query(q => q
                .Term(t => t
                    .Field("channelId.keyword")
                    .Value(id)
                )
            )
        );

        var blogger = response.Documents.FirstOrDefault();
        return blogger;
    }

    private static IEnumerable<string> NormalizeTags(IEnumerable<string>? tags)
    {
        return tags?
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase) ?? [];
    }

    private static RecommendedChannelModel MapToRecommendedChannelModel(RecommendedChannelDocument document)
    {
        return new RecommendedChannelModel
        {
            ChannelName = document.Name ?? string.Empty,
            ChannelUrl = document.ChannelUrl ?? string.Empty,
            CountryCode = document.CountryCode ?? string.Empty,
            Description = document.Description ?? string.Empty,
            AvatarUrl = document.AvatarUrl ?? string.Empty,
            EngagementRate = document.Statictics?.EngagementRate ?? 0,
            VideoCount = document.Statictics?.PerHalfYear?.VideoCount ?? 0,
            PostPerDay = document.Statictics?.PerHalfYear?.PostPerDay ?? 0,
            AvgLike = document.Statictics?.PerHalfYear?.AvgLike ?? 0,
            AvgView = document.Statictics?.PerHalfYear?.AvgView ?? 0,
            AvgComment = document.Statictics?.PerHalfYear?.AvgComment ?? 0
        };
    }
}

public class RecommendedChannelDocument
{
    public string? Name { get; set; }
    public string? ChannelUrl { get; set; }
    public string? CountryCode { get; set; }
    public string? Description { get; set; }
    public string? AvatarUrl { get; set; }
    public RecommendedChannelStaticticsDocument? Statictics { get; set; }
}

public class RecommendedChannelStaticticsDocument
{
    public float EngagementRate { get; set; }
    public RecommendedChannelPerHalfYearDocument? PerHalfYear { get; set; }
}

public class RecommendedChannelPerHalfYearDocument
{
    public int VideoCount { get; set; }
    public float PostPerDay { get; set; }
    public int AvgLike { get; set; }
    public int AvgView { get; set; }
    public int AvgComment { get; set; }
}
