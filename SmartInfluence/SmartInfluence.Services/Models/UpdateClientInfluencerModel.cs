using SmartInfluence.Data.Entities;

namespace SmartInfluence.Services.Models;

public class UpdateClientInfluencerModel
{
    public int? TotalScore { get; set; }
    
    public decimal? BrandFitScore { get; set; }
    
    public string? AiReview { get; set; }
    public Status? Status { get; set; }
    public decimal? PredictedEngagement { get; set; }
}