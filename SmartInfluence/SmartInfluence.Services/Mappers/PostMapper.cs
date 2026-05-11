using SmartInfluence.Data.Entities;
using SmartInfluence.Services.Models;

namespace SmartInfluence.Services.Mappers;

public static class PostMapper
{
    public static PostResponseModel MapToResponseModel(Post post)
    {
        return new PostResponseModel
        {
            Id = post.Id,
            InfluencerId = post.InfluencerId,
            PlatformPostId = post.PlatformPostId,
            ContentType = post.ContentType,
            Caption = post.Caption,
            Hashtags = post.Hashtags,
            Views = post.Views,
            Likes = post.Likes,
            Comments = post.Comments,
            Shares = post.Shares,
            PostedAt = post.PostedAt
        };
    }
}
