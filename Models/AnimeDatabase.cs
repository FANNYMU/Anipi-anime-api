using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace Anipi.Models;

public class AnimeDatabase
{
    [JsonPropertyName("$schema")]
    public string? Schema { get; set; }
    
    public License? License { get; set; }
    
    public string? Repository { get; set; }
    
    [JsonPropertyName("scoreRange")]
    public ScoreRange? ScoreRange { get; set; }
    
    [JsonPropertyName("lastUpdate")]
    public string? LastUpdate { get; set; }
    
    public List<Anime>? Data { get; set; }
}

public class License
{
    public string? Name { get; set; }
    public string? Url { get; set; }
}

public class ScoreRange
{
    [JsonPropertyName("minInclusive")]
    public double MinInclusive { get; set; }
    
    [JsonPropertyName("maxInclusive")]
    public double MaxInclusive { get; set; }
}
