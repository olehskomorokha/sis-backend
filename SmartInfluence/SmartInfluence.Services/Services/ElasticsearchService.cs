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

    public async Task<List<object>> GetByFilters(FiltersModel model)
    {
        var size = Math.Clamp(model.ResponseCount ?? 50, 1, 500);
        var tags = model.Tags?
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? [];

        var hasCountry = !string.IsNullOrWhiteSpace(model.Country);
        var hasFollowersRange = model.MinFollowersCount.HasValue || model.MaxFollowersCount.HasValue;
        var hasTags = tags.Count > 0;
        var hasFilters = hasCountry || hasFollowersRange || hasTags;

        var filters = new List<Action<QueryDescriptor<object>>>();

        if (hasCountry)
        {
            filters.Add(q => q.Term(t => t
                .Field("countryCode.keyword")
                .Value(model.Country!)
                .CaseInsensitive(true)));
        }

        if (hasFollowersRange)
        {
            filters.Add(q => q.Range(r => r
                .NumberRange(nr =>
                {
                    nr.Field("fields.subscriberCount");

                    if (model.MinFollowersCount.HasValue)
                    {
                        nr.Gte(model.MinFollowersCount.Value);
                    }

                    if (model.MaxFollowersCount.HasValue)
                    {
                        nr.Lte(model.MaxFollowersCount.Value);
                    }
                })));
        }

        if (hasTags)
        {
            var tagWildcardFilters = tags
                .Select<string, Action<QueryDescriptor<object>>>(tag =>
                    q => q.Bool(b => b
                        .Should(
                            sq => sq.Wildcard(w => w
                                .Field("interests.channelTags")
                                .Value($"*{tag}*")
                                .CaseInsensitive(true)),
                            sq => sq.Wildcard(w => w
                                .Field("interests.videoTags")
                                .Value($"*{tag}*")
                                .CaseInsensitive(true)))
                        .MinimumShouldMatch(1)))
                .ToArray();

            filters.Add(q => q.Bool(bb => bb
                .Should(tagWildcardFilters)
                .MinimumShouldMatch(1)));
        }

        var response = await _client.SearchAsync<object>(s =>
        {
            s.Index(YoutuberIndex).Size(size);

            if (!hasFilters)
            {
                s.Query(q => q.MatchAll(_ => { }));
                return;
            }

            s.Query(q => q.Bool(b => b.Filter(filters.ToArray())));
        });

        if (!response.IsValidResponse)
        {
            return [];
        }

        return response.Documents.ToList();
    }

    public async Task<object> GetById(string id)
    {
        var response = await _client.SearchAsync<object>(s => s
            .Index("blogger")
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
}
