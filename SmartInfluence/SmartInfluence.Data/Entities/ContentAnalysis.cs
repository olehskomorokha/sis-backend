namespace SmartInfluence.Data.Entities;

public class ContentAnalysis
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Topics { get; set; } = string.Empty;
    public string Sentiment { get; set; } = string.Empty;
    public string BrandMentions { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public decimal ToxicityScore { get; set; }

    public Post? Post { get; set; }
}
