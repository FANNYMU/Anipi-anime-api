using System.Text.Json.Serialization;

namespace Anipi.Models;

public class Anime
{
    public string? Title { get; set; }
    public string? Type { get; set; }
    public int? Episodes { get; set; }
    public string? Status { get; set; }
    
    [JsonPropertyName("animeSeason")]
    public AnimeSeason? AnimeSeason { get; set; }
    
    public string? Picture { get; set; }
    public string? Thumbnail { get; set; }
    
    public List<string>? Synonyms { get; set; }
    public List<string>? Relations { get; set; }
    public List<string>? Tags { get; set; }
    
    [JsonPropertyName("sources")]
    public List<string>? Sources { get; set; }
    
    public AnimeScore? Score { get; set; }
    
    public AnimeDuration? Duration { get; set; }
}

public class AnimeSeason
{
    public int? Year { get; set; }
    public string? Season { get; set; }
}

public class AnimeScore
{
    [JsonPropertyName("arithmeticMean")]
    public double? ArithmeticMean { get; set; }
    
    [JsonPropertyName("arithmeticGeometricMean")]
    public double? ArithmeticGeometricMean { get; set; }
    
    [JsonPropertyName("median")]
    public double? Median { get; set; }
}

public class AnimeDuration
{
    public int? Value { get; set; }
    public string? Unit { get; set; }
}
