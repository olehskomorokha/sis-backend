namespace SmartInfluence.Services.Models;

public class VideoDetailModel
{
    public string VideoId { get; set; } = string.Empty;
    public string ChannelId { get; set; } = string.Empty;
    public string ChannelTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PublishedAt { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string TopicCategories { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string DefaultLanguage { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public bool LicensedContent { get; set; }
    public string UploadStatus { get; set; } = string.Empty;
    public ulong? ViewCount { get; set; }
    public ulong? LikeCount { get; set; }
    public ulong? FavoriteCount { get; set; }
    public ulong? CommentCount { get; set; }
}
