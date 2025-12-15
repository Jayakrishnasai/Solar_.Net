using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SolarSystem.Api.Data;

namespace SolarSystem.Api.Services;

/// <summary>
/// Service to fetch data from NASA APIs
/// </summary>
public class NasaApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NasaApiService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;

    public NasaApiService(
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory scopeFactory,
        ILogger<NasaApiService> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
        _apiKey = configuration["NasaApi:ApiKey"] ?? "DEMO_KEY";
    }

    /// <summary>
    /// Fetch Astronomy Picture of the Day
    /// </summary>
    public async Task<ApodResponse?> GetAstronomyPictureOfDay(DateTime? date = null)
    {
        var client = _httpClientFactory.CreateClient("NasaApi");
        
        var dateParam = date?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd");
        var url = $"planetary/apod?api_key={_apiKey}&date={dateParam}";

        try
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                await StoreSnapshotAsync("NASA-APOD", url, json, true);
                
                return JsonSerializer.Deserialize<ApodResponse>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            _logger.LogWarning("NASA APOD returned {Status}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching APOD");
            return null;
        }
    }

    /// <summary>
    /// Fetch Near Earth Objects for a date range
    /// </summary>
    public async Task<NeoResponse?> GetNearEarthObjects(DateTime startDate, DateTime endDate)
    {
        var client = _httpClientFactory.CreateClient("NasaApi");
        
        var start = startDate.ToString("yyyy-MM-dd");
        var end = endDate.ToString("yyyy-MM-dd");
        var url = $"neo/rest/v1/feed?start_date={start}&end_date={end}&api_key={_apiKey}";

        try
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                await StoreSnapshotAsync("NASA-NEO", url, json, true);
                
                return JsonSerializer.Deserialize<NeoResponse>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            _logger.LogWarning("NASA NEO returned {Status}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching NEO data");
            return null;
        }
    }

    /// <summary>
    /// Fetch Mars Rover Photos
    /// </summary>
    public async Task<MarsPhotosResponse?> GetMarsRoverPhotos(string rover = "curiosity", int sol = 1000)
    {
        var client = _httpClientFactory.CreateClient("NasaApi");
        var url = $"mars-photos/api/v1/rovers/{rover}/photos?sol={sol}&api_key={_apiKey}";

        try
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                await StoreSnapshotAsync("NASA-Mars", url, json, true);
                
                return JsonSerializer.Deserialize<MarsPhotosResponse>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Mars photos");
            return null;
        }
    }

    /// <summary>
    /// Fetch NASA Image Library search results
    /// </summary>
    public async Task<NasaImageSearchResponse?> SearchImages(string query, int page = 1)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("https://images-api.nasa.gov/");
        
        var url = $"search?q={Uri.EscapeDataString(query)}&media_type=image&page={page}";

        try
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                await StoreSnapshotAsync("NASA-Images", url, json.Length > 50000 ? json[..50000] : json, true);
                
                return JsonSerializer.Deserialize<NasaImageSearchResponse>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching NASA images");
            return null;
        }
    }

    /// <summary>
    /// Fetch all available NASA data and store in database
    /// </summary>
    public async Task<NasaDataSummary> FetchAllNasaData()
    {
        var summary = new NasaDataSummary { FetchedAt = DateTime.UtcNow };

        // 1. Astronomy Picture of the Day
        _logger.LogInformation("Fetching APOD...");
        var apod = await GetAstronomyPictureOfDay();
        if (apod != null)
        {
            summary.ApodTitle = apod.Title;
            summary.ApodUrl = apod.Url;
            summary.ApodDate = apod.Date;
            summary.ApodFetched = true;
        }

        // 2. Near Earth Objects (last 7 days)
        _logger.LogInformation("Fetching NEO data...");
        var neo = await GetNearEarthObjects(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        if (neo != null)
        {
            summary.NeoCount = neo.ElementCount;
            summary.NeoFetched = true;
        }

        // 3. Mars Photos
        _logger.LogInformation("Fetching Mars photos...");
        var mars = await GetMarsRoverPhotos();
        if (mars?.Photos != null)
        {
            summary.MarsPhotoCount = mars.Photos.Count;
            summary.MarsFetched = true;
        }

        // 4. Planet Images
        _logger.LogInformation("Fetching planet images...");
        var planets = new[] { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };
        foreach (var planet in planets)
        {
            var images = await SearchImages($"{planet} planet");
            if (images?.Collection?.Items != null)
            {
                summary.PlanetImageCounts[planet] = images.Collection.Metadata?.TotalHits ?? images.Collection.Items.Count;
            }
            await Task.Delay(200); // Rate limiting
        }

        summary.Success = true;
        _logger.LogInformation("NASA data fetch complete. APOD: {Apod}, NEO: {Neo}, Mars: {Mars}", 
            summary.ApodFetched, summary.NeoCount, summary.MarsPhotoCount);

        return summary;
    }

    private async Task StoreSnapshotAsync(string source, string endpoint, string data, bool isValid)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SolarSystemDbContext>();
            
            context.ApiSnapshots.Add(new Shared.Models.ApiSnapshot
            {
                Source = source,
                Endpoint = endpoint.Length > 500 ? endpoint[..500] : endpoint,
                RawData = data.Length > 50000 ? data[..50000] : data,
                FetchedAt = DateTime.UtcNow,
                IsValid = isValid
            });
            
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing NASA snapshot");
        }
    }
}

// NASA API Response Models
public class ApodResponse
{
    public string? Title { get; set; }
    public string? Explanation { get; set; }
    public string? Url { get; set; }
    public string? HdUrl { get; set; }
    public string? Date { get; set; }
    public string? MediaType { get; set; }
    public string? Copyright { get; set; }
}

public class NeoResponse
{
    [JsonPropertyName("element_count")]
    public int ElementCount { get; set; }
    
    [JsonPropertyName("near_earth_objects")]
    public Dictionary<string, List<NeoObject>>? NearEarthObjects { get; set; }
}

public class NeoObject
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    
    [JsonPropertyName("absolute_magnitude_h")]
    public double AbsoluteMagnitude { get; set; }
    
    [JsonPropertyName("is_potentially_hazardous_asteroid")]
    public bool IsPotentiallyHazardous { get; set; }
    
    [JsonPropertyName("estimated_diameter")]
    public EstimatedDiameter? EstimatedDiameter { get; set; }
}

public class EstimatedDiameter
{
    public DiameterRange? Kilometers { get; set; }
    public DiameterRange? Meters { get; set; }
}

public class DiameterRange
{
    [JsonPropertyName("estimated_diameter_min")]
    public double Min { get; set; }
    
    [JsonPropertyName("estimated_diameter_max")]
    public double Max { get; set; }
}

public class MarsPhotosResponse
{
    public List<MarsPhoto>? Photos { get; set; }
}

public class MarsPhoto
{
    public int Id { get; set; }
    public int Sol { get; set; }
    
    [JsonPropertyName("img_src")]
    public string? ImgSrc { get; set; }
    
    [JsonPropertyName("earth_date")]
    public string? EarthDate { get; set; }
    
    public MarsCamera? Camera { get; set; }
    public MarsRover? Rover { get; set; }
}

public class MarsCamera
{
    public string? Name { get; set; }
    
    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }
}

public class MarsRover
{
    public string? Name { get; set; }
    public string? Status { get; set; }
}

public class NasaImageSearchResponse
{
    public ImageCollection? Collection { get; set; }
}

public class ImageCollection
{
    public List<ImageItem>? Items { get; set; }
    public CollectionMetadata? Metadata { get; set; }
}

public class CollectionMetadata
{
    [JsonPropertyName("total_hits")]
    public int TotalHits { get; set; }
}

public class ImageItem
{
    public string? Href { get; set; }
    public List<ImageData>? Data { get; set; }
    public List<ImageLink>? Links { get; set; }
}

public class ImageData
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    
    [JsonPropertyName("date_created")]
    public string? DateCreated { get; set; }
    
    public string? Center { get; set; }
}

public class ImageLink
{
    public string? Href { get; set; }
    public string? Rel { get; set; }
}

public class NasaDataSummary
{
    public DateTime FetchedAt { get; set; }
    public bool Success { get; set; }
    
    // APOD
    public bool ApodFetched { get; set; }
    public string? ApodTitle { get; set; }
    public string? ApodUrl { get; set; }
    public string? ApodDate { get; set; }
    
    // NEO
    public bool NeoFetched { get; set; }
    public int NeoCount { get; set; }
    
    // Mars
    public bool MarsFetched { get; set; }
    public int MarsPhotoCount { get; set; }
    
    // Planet Images
    public Dictionary<string, int> PlanetImageCounts { get; set; } = new();
}
