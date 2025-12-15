using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SolarSystem.Api.Data;
using SolarSystem.Shared.Models;

namespace SolarSystem.Api.Services;

/// <summary>
/// Service to fetch data from Le Système Solaire API and sync to database
/// </summary>
public class SolarSystemApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SolarSystemApiService> _logger;
    private readonly IConfiguration _configuration;

    public SolarSystemApiService(
        IHttpClientFactory httpClientFactory,
        IServiceScopeFactory scopeFactory,
        ILogger<SolarSystemApiService> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Fetch all celestial bodies from the API
    /// </summary>
    public async Task<ApiResponse?> FetchBodiesFromApiAsync()
    {
        var client = _httpClientFactory.CreateClient("SolarSystemApi");
        
        // Add API key if configured
        var apiKey = _configuration["SolarSystemApi:ApiKey"];
        if (!string.IsNullOrEmpty(apiKey))
        {
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        }

        try
        {
            var response = await client.GetAsync("bodies");
            
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                
                // Store snapshot
                await StoreSnapshotAsync("SolarSystemApi", "/bodies", json, true);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                return JsonSerializer.Deserialize<ApiResponse>(json, options);
            }
            else
            {
                _logger.LogWarning("API returned {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching from Solar System API");
            await StoreSnapshotAsync("SolarSystemApi", "/bodies", ex.Message, false);
            return null;
        }
    }

    /// <summary>
    /// Sync API data to database
    /// </summary>
    public async Task SyncToDatabase()
    {
        _logger.LogInformation("Starting API sync to database...");
        
        var apiData = await FetchBodiesFromApiAsync();
        
        if (apiData?.Bodies == null || !apiData.Bodies.Any())
        {
            _logger.LogWarning("No data from API, keeping existing data");
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SolarSystemDbContext>();

        foreach (var body in apiData.Bodies.Where(b => b.IsPlanet || b.BodyType == "Star"))
        {
            try
            {
                var existing = await context.CelestialBodies
                    .FirstOrDefaultAsync(c => c.ApiId == body.Id);

                if (existing != null)
                {
                    // Update existing
                    MapApiToEntity(body, existing);
                    existing.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new
                    var entity = new CelestialBody { ApiId = body.Id };
                    MapApiToEntity(body, entity);
                    context.CelestialBodies.Add(entity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing body {Id}", body.Id);
            }
        }

        await context.SaveChangesAsync();
        _logger.LogInformation("Sync completed. Processed {Count} bodies", apiData.Bodies.Count);
    }

    private void MapApiToEntity(ApiBody api, CelestialBody entity)
    {
        entity.Name = api.Name ?? "";
        entity.EnglishName = api.EnglishName ?? "";
        entity.BodyType = api.BodyType ?? "";
        entity.IsPlanet = api.IsPlanet;
        entity.Mass = api.Mass?.MassValue ?? 0;
        entity.MassExponent = api.Mass?.MassExponent ?? 0;
        entity.MeanRadius = api.MeanRadius ?? 0;
        entity.EquatorialRadius = api.EquaRadius ?? 0;
        entity.PolarRadius = api.PolarRadius ?? 0;
        entity.Density = api.Density ?? 0;
        entity.Gravity = api.Gravity ?? 0;
        entity.EscapeSpeed = api.Escape ?? 0;
        entity.MeanTemperature = api.AvgTemp ?? 0;
        entity.SiderealRotation = api.SideralRotation ?? 0;
        entity.AxialTilt = api.AxialTilt ?? 0;
        entity.Flattening = api.Flattening ?? 0;

        // Update or create orbit
        if (entity.Orbit == null)
        {
            entity.Orbit = new Orbit();
        }
        entity.Orbit.SemimajorAxis = api.SemimajorAxis ?? 0;
        entity.Orbit.Perihelion = api.Perihelion ?? 0;
        entity.Orbit.Aphelion = api.Aphelion ?? 0;
        entity.Orbit.Eccentricity = api.Eccentricity ?? 0;
        entity.Orbit.Inclination = api.Inclination ?? 0;
        entity.Orbit.SiderealOrbit = api.SideralOrbit ?? 0;
        entity.Orbit.OrbitalPeriod = api.SideralOrbit ?? 0;
        entity.Orbit.ArgumentOfPerihelion = api.ArgPeriapsis ?? 0;
        entity.Orbit.LongitudeOfAscendingNode = api.LongAscNode ?? 0;
        entity.Orbit.MeanAnomaly = api.MainAnomaly ?? 0;
    }

    private async Task StoreSnapshotAsync(string source, string endpoint, string data, bool isValid)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SolarSystemDbContext>();
            
            context.ApiSnapshots.Add(new ApiSnapshot
            {
                Source = source,
                Endpoint = endpoint,
                RawData = data.Length > 50000 ? data[..50000] : data,
                FetchedAt = DateTime.UtcNow,
                IsValid = isValid
            });
            
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing snapshot");
        }
    }
}

// API Response Models (matching Le Système Solaire API schema)
public class ApiResponse
{
    [JsonPropertyName("bodies")]
    public List<ApiBody> Bodies { get; set; } = new();
}

public class ApiBody
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("englishName")]
    public string? EnglishName { get; set; }
    
    [JsonPropertyName("isPlanet")]
    public bool IsPlanet { get; set; }
    
    [JsonPropertyName("bodyType")]
    public string? BodyType { get; set; }
    
    [JsonPropertyName("semimajorAxis")]
    public double? SemimajorAxis { get; set; }
    
    [JsonPropertyName("perihelion")]
    public double? Perihelion { get; set; }
    
    [JsonPropertyName("aphelion")]
    public double? Aphelion { get; set; }
    
    [JsonPropertyName("eccentricity")]
    public double? Eccentricity { get; set; }
    
    [JsonPropertyName("inclination")]
    public double? Inclination { get; set; }
    
    [JsonPropertyName("mass")]
    public ApiMass? Mass { get; set; }
    
    [JsonPropertyName("density")]
    public double? Density { get; set; }
    
    [JsonPropertyName("gravity")]
    public double? Gravity { get; set; }
    
    [JsonPropertyName("escape")]
    public double? Escape { get; set; }
    
    [JsonPropertyName("meanRadius")]
    public double? MeanRadius { get; set; }
    
    [JsonPropertyName("equaRadius")]
    public double? EquaRadius { get; set; }
    
    [JsonPropertyName("polarRadius")]
    public double? PolarRadius { get; set; }
    
    [JsonPropertyName("flattening")]
    public double? Flattening { get; set; }
    
    [JsonPropertyName("sideralOrbit")]
    public double? SideralOrbit { get; set; }
    
    [JsonPropertyName("sideralRotation")]
    public double? SideralRotation { get; set; }
    
    [JsonPropertyName("axialTilt")]
    public double? AxialTilt { get; set; }
    
    [JsonPropertyName("avgTemp")]
    public double? AvgTemp { get; set; }
    
    [JsonPropertyName("mainAnomaly")]
    public double? MainAnomaly { get; set; }
    
    [JsonPropertyName("argPeriapsis")]
    public double? ArgPeriapsis { get; set; }
    
    [JsonPropertyName("longAscNode")]
    public double? LongAscNode { get; set; }
    
    [JsonPropertyName("moons")]
    public List<ApiMoonRef>? Moons { get; set; }
}

public class ApiMass
{
    [JsonPropertyName("massValue")]
    public double MassValue { get; set; }
    
    [JsonPropertyName("massExponent")]
    public int MassExponent { get; set; }
}

public class ApiMoonRef
{
    [JsonPropertyName("moon")]
    public string? Moon { get; set; }
    
    [JsonPropertyName("rel")]
    public string? Rel { get; set; }
}
