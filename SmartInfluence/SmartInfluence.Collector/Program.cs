using System.Text;
using System.Text.Json;
using SmartInfluence.Collector.YouTube;

Console.OutputEncoding = Encoding.UTF8;

var baseDirectory = AppContext.BaseDirectory;
var projectDirectory = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", ".."));

var apiKey = ResolveYouTubeApiKey(projectDirectory);
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("YouTube API key was not found.");
    Console.WriteLine("Expected one of:");
    Console.WriteLine("  1. Environment variable YOUTUBE_API_KEY");
    Console.WriteLine("  2. SmartInfluence.Collector/appsettings.json -> YouTube:ApiKey");
    Console.WriteLine("  3. SmartInfluence.Api/appsettings.Development.json -> YouTube:ApiKey");
    return;
}

var count = TryParseCount(args);
var outputPath = Path.Combine(projectDirectory, "Output", "ukrainian-youtube-bloggers.json");

Console.WriteLine($"Starting YouTube export. Count: {count}");
Console.WriteLine($"Output file: {outputPath}");

try
{
    var channels = await YouTubeApi.ExportUkrainianBloggersToJsonAsync(apiKey, count, outputPath);

    Console.WriteLine();
    Console.WriteLine($"Export finished. Saved {channels.Count} channels.");
    Console.WriteLine();

    foreach (var channel in channels)
    {
        Console.WriteLine($"- {channel.Title}");
        Console.WriteLine($"  ChannelId: {channel.ChannelId}");
        Console.WriteLine($"  Subscribers: {FormatNumber(channel.SubscriberCount)}");
        Console.WriteLine($"  Videos: {FormatNumber(channel.VideoCount)}");
        Console.WriteLine($"  Views: {FormatNumber(channel.ViewCount)}");
        Console.WriteLine($"  Country: {channel.Country ?? "n/a"}");
        Console.WriteLine($"  Query: {channel.SourceQuery}");
        Console.WriteLine($"  Url: {channel.ChannelUrl}");
        Console.WriteLine();
    }
}
catch (Exception exception)
{
    Console.WriteLine("YouTube export failed.");
    Console.WriteLine(exception.Message);
}

static int TryParseCount(string[] args)
{
    if (args.Length > 0 && int.TryParse(args[0], out var count) && count > 0)
    {
        return count;
    }

    return 10;
}

static string? ResolveYouTubeApiKey(string projectDirectory)
{
    var envApiKey = Environment.GetEnvironmentVariable("YOUTUBE_API_KEY");
    if (!string.IsNullOrWhiteSpace(envApiKey))
    {
        return envApiKey;
    }

    var collectorConfigPath = Path.Combine(projectDirectory, "appsettings.json");
    var collectorDevConfigPath = Path.Combine(projectDirectory, "appsettings.Development.json");
    var apiProjectDirectory = Path.GetFullPath(Path.Combine(projectDirectory, "..", "SmartInfluence.Api"));
    var apiConfigPath = Path.Combine(apiProjectDirectory, "appsettings.json");
    var apiDevConfigPath = Path.Combine(apiProjectDirectory, "appsettings.Development.json");

    return ReadApiKeyFromJson(collectorDevConfigPath)
        ?? ReadApiKeyFromJson(collectorConfigPath)
        ?? ReadApiKeyFromJson(apiDevConfigPath)
        ?? ReadApiKeyFromJson(apiConfigPath);
}

static string? ReadApiKeyFromJson(string path)
{
    if (!File.Exists(path))
    {
        return null;
    }

    using var stream = File.OpenRead(path);
    using var document = JsonDocument.Parse(stream);

    if (!document.RootElement.TryGetProperty("YouTube", out var youTubeSection))
    {
        return null;
    }

    if (!youTubeSection.TryGetProperty("ApiKey", out var apiKeyElement))
    {
        return null;
    }

    return apiKeyElement.GetString();
}

static string FormatNumber(ulong? value)
{
    return value?.ToString("N0") ?? "n/a";
}
