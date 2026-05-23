using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SmartInfluence.Services.Interfaces;
using SmartInfluence.Services.Mappers;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Services;

public class ElasticsearchService : IElasticsearchService
{
    private const string BloggerIndex = "blogger";
    private const string VideoDetailIndex = "youtube-video-details";

    private readonly ElasticsearchClient _client;
    
    public ElasticsearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<List<string>> GetAllBloggerTags()
    {
        var response = await _client.SearchAsync<object>(s => s
            .Index(BloggerIndex)
            .Size(0)
            .Aggregations(aggs => aggs
                .Add("unique_topic_categories", a => a
                    .Terms(t => t
                        .Field("topicCategories.keyword")
                        .Size(1000)
                    )
                )
            )
        );

        var aggregation = response.Aggregations?
            .GetStringTerms("unique_topic_categories");

        if (aggregation?.Buckets == null)
        {
            return [];
        }

        return aggregation.Buckets
            .Select(b => b.Key.ToString())
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .SelectMany(key => key!.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<List<InfluencerSearchModel>> GetByFilters(FiltersModel model)
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

        var filters = new List<Action<QueryDescriptor<BloggerDocument>>>();

        if (hasCountry)
        {
            filters.Add(q => q.Term(t => t
                .Field("country")
                .Value(model.Country!)
                .CaseInsensitive(true)));
        }

        if (hasFollowersRange)
        {
            filters.Add(q => q.Range(r => r
                .NumberRange(nr =>
                {
                    nr.Field("subscriberCount");

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
                .Select<string, Action<QueryDescriptor<BloggerDocument>>>(tag =>
                    q => q.Wildcard(w => w
                        .Field("topicCategories")
                        .Value($"*{tag}*")
                        .CaseInsensitive(true)))
                .ToArray();

            filters.Add(q => q.Bool(bb => bb
                .Should(tagWildcardFilters)
                .MinimumShouldMatch(1)));
        }

        var response = await _client.SearchAsync<BloggerDocument>(s =>
        {
            s.Index(BloggerIndex).Size(size);

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

        return response.Documents
            .Select(ElasticsearchMapper.MapToInfluencerSearchModel)
            .ToList();
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
    public async Task<List<VideoDetailModel>> GetAllVideoDetailsByChannel(string channelId)
    {
        var response = await _client.SearchAsync<VideoDetailModel>(s => s
            .Index("youtube-video-details")
            .Query(q => q
                .Term(t => t
                    .Field("channelId.keyword")
                    .Value("UCuf5y77EhiKWNxzn9Y_Wa_Q"))
            )
        );
        
        return response.Documents.ToList();
    }
}
