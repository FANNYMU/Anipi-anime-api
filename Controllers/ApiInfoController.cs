using Microsoft.AspNetCore.Mvc;
using Anipi.Models;

namespace Anipi.Controllers;

[ApiController]
[Route("api")]
public class ApiInfoController : ControllerBase
{
    /// <summary>
    /// Get API information and available versions
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<ApiInfo>), StatusCodes.Status200OK)]
    public IActionResult GetApiInfo()
    {
        var apiInfo = new ApiInfo
        {
            Name = "Anipi - Professional Anime API",
            Description = "A comprehensive API for anime information",
            Versions = new List<ApiVersion>
            {
                new ApiVersion
                {
                    Version = "v1",
                    Status = "stable",
                    BaseUrl = "/api/v1"
                }
            }
        };
        
        return Ok(new ApiResponse<ApiInfo>
        {
            Data = apiInfo
        });
    }
}

public class ApiInfo
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<ApiVersion>? Versions { get; set; }
}

public class ApiVersion
{
    public string? Version { get; set; }
    public string? Status { get; set; }
    public string? BaseUrl { get; set; }
}
