using Microsoft.AspNetCore.Mvc;
using SolarSystem.Api.Services;

namespace SolarSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NasaController : ControllerBase
{
    private readonly NasaApiService _nasaService;
    private readonly ILogger<NasaController> _logger;

    public NasaController(NasaApiService nasaService, ILogger<NasaController> logger)
    {
        _nasaService = nasaService;
        _logger = logger;
    }

    /// <summary>
    /// Get Astronomy Picture of the Day
    /// </summary>
    [HttpGet("apod")]
    public async Task<ActionResult<ApodResponse>> GetApod([FromQuery] string? date = null)
    {
        DateTime? parsedDate = null;
        if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var d))
        {
            parsedDate = d;
        }

        var result = await _nasaService.GetAstronomyPictureOfDay(parsedDate);
        
        if (result == null)
            return NotFound(new { message = "Could not fetch APOD" });
            
        return Ok(result);
    }

    /// <summary>
    /// Get Near Earth Objects
    /// </summary>
    [HttpGet("neo")]
    public async Task<ActionResult<NeoResponse>> GetNeo(
        [FromQuery] string? startDate = null, 
        [FromQuery] string? endDate = null)
    {
        var start = DateTime.TryParse(startDate, out var s) ? s : DateTime.UtcNow.AddDays(-7);
        var end = DateTime.TryParse(endDate, out var e) ? e : DateTime.UtcNow;

        var result = await _nasaService.GetNearEarthObjects(start, end);
        
        if (result == null)
            return NotFound(new { message = "Could not fetch NEO data" });
            
        return Ok(result);
    }

    /// <summary>
    /// Get Mars Rover Photos
    /// </summary>
    [HttpGet("mars")]
    public async Task<ActionResult<MarsPhotosResponse>> GetMarsPhotos(
        [FromQuery] string rover = "curiosity",
        [FromQuery] int sol = 1000)
    {
        var result = await _nasaService.GetMarsRoverPhotos(rover, sol);
        
        if (result == null)
            return NotFound(new { message = "Could not fetch Mars photos" });
            
        return Ok(result);
    }

    /// <summary>
    /// Search NASA Image Library
    /// </summary>
    [HttpGet("images")]
    public async Task<ActionResult<NasaImageSearchResponse>> SearchImages(
        [FromQuery] string q = "solar system",
        [FromQuery] int page = 1)
    {
        var result = await _nasaService.SearchImages(q, page);
        
        if (result == null)
            return NotFound(new { message = "Could not search images" });
            
        return Ok(result);
    }

    /// <summary>
    /// Fetch all NASA data and return summary
    /// </summary>
    [HttpPost("sync")]
    public async Task<ActionResult<NasaDataSummary>> SyncAllData()
    {
        _logger.LogInformation("Manual NASA data sync triggered");
        var result = await _nasaService.FetchAllNasaData();
        return Ok(result);
    }

    /// <summary>
    /// Get summary of available NASA data types
    /// </summary>
    [HttpGet("info")]
    public ActionResult GetInfo()
    {
        return Ok(new
        {
            apiName = "NASA Open APIs",
            endpoints = new[]
            {
                new { path = "/api/nasa/apod", description = "Astronomy Picture of the Day", params_ = "date (optional)" },
                new { path = "/api/nasa/neo", description = "Near Earth Objects", params_ = "startDate, endDate" },
                new { path = "/api/nasa/mars", description = "Mars Rover Photos", params_ = "rover, sol" },
                new { path = "/api/nasa/images", description = "NASA Image Library Search", params_ = "q, page" },
                new { path = "/api/nasa/sync", description = "Sync all NASA data (POST)", params_ = "none" }
            },
            documentation = "https://api.nasa.gov/"
        });
    }
}
