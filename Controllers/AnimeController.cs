using Anipi.Models;
using Anipi.Services;
using Microsoft.AspNetCore.Mvc;

namespace Anipi.Controllers;

[ApiController]
[Route("api/v1")]
public class AnimeController : ControllerBase
{
    private readonly IAnimeService _animeService;
    private readonly ILogger<AnimeController> _logger;

    public AnimeController(IAnimeService animeService, ILogger<AnimeController> logger)
    {
        _animeService = animeService;
        _logger = logger;
    }

    /// <summary>
    /// Get a paginated list of anime with optional filtering and sorting
    /// </summary>
    [HttpGet("anime")]
    [ProducesResponseType(typeof(PaginatedResponse<Anime>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnime([FromQuery] AnimeQueryParameters parameters)
    {
        try
        {
            var result = await _animeService.GetAnimePaginatedAsync(parameters);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving anime list");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving anime data"
            });
        }
    }

    /// <summary>
    /// Get a specific anime by source URL
    /// </summary>
    [HttpGet("anime/source")]
    [ProducesResponseType(typeof(ApiResponse<Anime>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnimeBySource([FromQuery] string url)
    {
        try
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Source URL is required"
                });
            }
            
            var result = await _animeService.GetAnimeByIdAsync(url);
            
            if (!result.Success)
                return NotFound(result);
                
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving anime with source URL {Url}", url);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving anime data"
            });
        }
    }

    /// <summary>
    /// Get available anime types
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnimeTypes()
    {
        try
        {
            var database = await _animeService.GetFullDatabaseAsync();
            if (database?.Data == null)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to load anime database"
                });
            }

            var types = database.Data
                .Where(a => !string.IsNullOrEmpty(a.Type))
                .Select(a => a.Type!)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            return Ok(new ApiResponse<List<string>>
            {
                Data = types
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving anime types");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving anime types"
            });
        }
    }

    /// <summary>
    /// Get available anime statuses
    /// </summary>
    [HttpGet("statuses")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnimeStatuses()
    {
        try
        {
            var database = await _animeService.GetFullDatabaseAsync();
            if (database?.Data == null)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to load anime database"
                });
            }

            var statuses = database.Data
                .Where(a => !string.IsNullOrEmpty(a.Status))
                .Select(a => a.Status!)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            return Ok(new ApiResponse<List<string>>
            {
                Data = statuses
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving anime statuses");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving anime statuses"
            });
        }
    }

    /// <summary>
    /// Get available seasons
    /// </summary>
    [HttpGet("seasons")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnimeSeasons()
    {
        try
        {
            var database = await _animeService.GetFullDatabaseAsync();
            if (database?.Data == null)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to load anime database"
                });
            }

            var seasons = database.Data
                .Where(a => a.AnimeSeason?.Season != null)
                .Select(a => a.AnimeSeason!.Season!)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            return Ok(new ApiResponse<List<string>>
            {
                Data = seasons
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving anime seasons");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving anime seasons"
            });
        }
    }

    /// <summary>
    /// Get available years
    /// </summary>
    [HttpGet("years")]
    [ProducesResponseType(typeof(ApiResponse<List<int>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnimeYears()
    {
        try
        {
            var database = await _animeService.GetFullDatabaseAsync();
            if (database?.Data == null)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to load anime database"
                });
            }

            var years = database.Data
                .Where(a => a.AnimeSeason?.Year != null)
                .Select(a => a.AnimeSeason!.Year!.Value)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            return Ok(new ApiResponse<List<int>>
            {
                Data = years
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving anime years");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving anime years"
            });
        }
    }

    /// <summary>
    /// Get popular tags
    /// </summary>
    [HttpGet("tags")]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAnimeTags()
    {
        try
        {
            var database = await _animeService.GetFullDatabaseAsync();
            if (database?.Data == null)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to load anime database"
                });
            }

            var tags = database.Data
                .Where(a => a.Tags != null && a.Tags.Any())
                .SelectMany(a => a.Tags!)
                .GroupBy(t => t)
                .Select(g => new { Tag = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(100)
                .Select(g => g.Tag)
                .ToList();

            return Ok(new ApiResponse<List<string>>
            {
                Data = tags
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving anime tags");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving anime tags"
            });
        }
    }
}
