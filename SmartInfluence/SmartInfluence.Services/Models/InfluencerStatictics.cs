namespace SmartInfluence.Services.Models;

public class InfluencerStatistics
{
    public int PostCount { get; set; }
    public PerPostP50 PostPerPostP50 { get; set; }
}
public class PerPostP50
{
    public double Engagement { get; set; }
    public double AvgViews { get; set; }
    public double AvgLikes { get; set; }
}