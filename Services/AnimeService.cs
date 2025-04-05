using System.Text.Json;
using Anipi.Models;

namespace Anipi.Services;

public interface IAnimeService
{
    Task<AnimeDatabase?> GetFullDatabaseAsync();
    Task<PaginatedResponse<Anime>> GetAnimePaginatedAsync(AnimeQueryParameters parameters);
    Task<ApiResponse<Anime>> GetAnimeByIdAsync(string sourceUrl);
}

public class AnimeService : IAnimeService
{
    private readonly ILogger<AnimeService> _logger;
    private readonly string _dataFilePath;
    private AnimeDatabase? _cachedDatabase;

    public AnimeService(ILogger<AnimeService> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _dataFilePath = Path.Combine(environment.ContentRootPath, "data", "anime-offline-database.json");
        _logger.LogInformation("Anime database file path: {FilePath}", _dataFilePath);
    }

    public async Task<AnimeDatabase?> GetFullDatabaseAsync()
    {
        try
        {
            if (_cachedDatabase != null)
                return _cachedDatabase;

            if (!File.Exists(_dataFilePath))
            {
                _logger.LogError("Anime database file not found at {FilePath}", _dataFilePath);
                return null;
            }

            var jsonData = await File.ReadAllTextAsync(_dataFilePath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            
            _cachedDatabase = JsonSerializer.Deserialize<AnimeDatabase>(jsonData, options);
            
            if (_cachedDatabase == null)
            {
                _logger.LogError("Failed to deserialize anime database");
                return null;
            }
            
            _logger.LogInformation("Successfully loaded anime database with {Count} entries", 
                _cachedDatabase.Data?.Count ?? 0);
                
            return _cachedDatabase;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading anime database: {Message}", ex.Message);
            return null;
        }
    }

    public async Task<PaginatedResponse<Anime>> GetAnimePaginatedAsync(AnimeQueryParameters parameters)
    {
        var database = await GetFullDatabaseAsync();
        if (database?.Data == null || !database.Data.Any())
        {
            _logger.LogWarning("No anime data available for pagination");
            return new PaginatedResponse<Anime>
            {
                Success = false,
                Message = "Failed to load anime database",
                Data = new List<Anime>()
            };
        }

        var query = database.Data.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(parameters.Title))
        {
            query = query.Where(a => a.Title != null && 
                a.Title.Contains(parameters.Title, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Type))
        {
            query = query.Where(a => a.Type != null && 
                a.Type.Equals(parameters.Type, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Status))
        {
            query = query.Where(a => a.Status != null && 
                a.Status.Equals(parameters.Status, StringComparison.OrdinalIgnoreCase));
        }

        if (parameters.Year.HasValue)
        {
            query = query.Where(a => a.AnimeSeason != null && a.AnimeSeason.Year == parameters.Year);
        }

        if (!string.IsNullOrWhiteSpace(parameters.Season))
        {
            query = query.Where(a => a.AnimeSeason != null && a.AnimeSeason.Season != null && 
                a.AnimeSeason.Season.Equals(parameters.Season, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Tag))
        {
            query = query.Where(a => a.Tags != null && 
                a.Tags.Any(t => t.Contains(parameters.Tag, StringComparison.OrdinalIgnoreCase)));
        }

        // Apply sorting
        if (!string.IsNullOrWhiteSpace(parameters.SortBy))
        {
            query = parameters.SortBy.ToLower() switch
            {
                "title" => parameters.SortDescending 
                    ? query.OrderByDescending(a => a.Title)
                    : query.OrderBy(a => a.Title),
                "score" => parameters.SortDescending 
                    ? query.OrderByDescending(a => a.Score != null ? a.Score.ArithmeticMean : 0)
                    : query.OrderBy(a => a.Score != null ? a.Score.ArithmeticMean : 0),
                "year" => parameters.SortDescending 
                    ? query.OrderByDescending(a => a.AnimeSeason != null ? a.AnimeSeason.Year : 0)
                    : query.OrderBy(a => a.AnimeSeason != null ? a.AnimeSeason.Year : 0),
                _ => query.OrderBy(a => a.Title)
            };
        }
        else
        {
            query = query.OrderBy(a => a.Title);
        }

        // Calculate pagination
        var totalCount = query.Count();
        var totalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize);
        
        var items = query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToList();

        return new PaginatedResponse<Anime>
        {
            Data = items,
            Pagination = new PaginationMetadata
            {
                Page = parameters.Page,
                PageSize = parameters.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            }
        };
    }

    public async Task<ApiResponse<Anime>> GetAnimeByIdAsync(string sourceUrl)
    {
        var database = await GetFullDatabaseAsync();
        if (database?.Data == null)
        {
            return new ApiResponse<Anime>
            {
                Success = false,
                Message = "Failed to load anime database"
            };
        }

        var anime = database.Data.FirstOrDefault(a => a.Sources != null && a.Sources.Contains(sourceUrl));
        if (anime == null)
        {
            return new ApiResponse<Anime>
            {
                Success = false,
                Message = $"Anime with source URL {sourceUrl} not found"
            };
        }

        return new ApiResponse<Anime>
        {
            Data = anime
        };
    }
}
