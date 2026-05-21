using System.Text;
using System.Text.Json;
using SmartInfluence.Collector.Extentions;
using SmartInfluence.Data.Entities;

namespace SmartInfluence.Collector.YouTube;

public sealed class YouTubeAdapter
{
    private readonly Settings _settings;
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyList<YouTubeApi.UkrainianYouTubeBloggerDto>> ReadBloggersFromJsonAsync(string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await using var stream = File.OpenRead(path);
        var bloggers = await JsonSerializer.DeserializeAsync<List<YouTubeApi.UkrainianYouTubeBloggerDto>>(stream, JsonSerializerOptions, cancellationToken);

        if (bloggers is null || bloggers.Count == 0)
        {
            return [];
        }

        return bloggers;
    }
}
