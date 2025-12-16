using Microsoft.AspNetCore.Mvc;

namespace SolarSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeoController : ControllerBase
{
    private readonly ILogger<GeoController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public GeoController(ILogger<GeoController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    /// <summary>
    /// Get all countries
    /// </summary>
    [HttpGet("countries")]
    public ActionResult<List<GeoLocationDto>> GetCountries()
    {
        var countries = new List<GeoLocationDto>
        {
            new GeoLocationDto { Id = 1, Name = "India", Type = "Country", Lat = 20.5937, Lon = 78.9629, Population = 1400000000, Altitude = 5000000 },
            new GeoLocationDto { Id = 2, Name = "United States", Type = "Country", Lat = 37.0902, Lon = -95.7129, Population = 331000000, Altitude = 8000000 },
            new GeoLocationDto { Id = 3, Name = "United Kingdom", Type = "Country", Lat = 55.3781, Lon = -3.4360, Population = 67000000, Altitude = 3000000 },
            new GeoLocationDto { Id = 4, Name = "Australia", Type = "Country", Lat = -25.2744, Lon = 133.7751, Population = 26000000, Altitude = 6000000 },
            new GeoLocationDto { Id = 5, Name = "Japan", Type = "Country", Lat = 36.2048, Lon = 138.2529, Population = 125000000, Altitude = 3000000 }
        };

        return Ok(countries);
    }

    /// <summary>
    /// Get states by country
    /// </summary>
    [HttpGet("states/{countryId}")]
    public ActionResult<List<GeoLocationDto>> GetStates(int countryId)
    {
        // India states
        if (countryId == 1)
        {
            var states = new List<GeoLocationDto>
            {
                new GeoLocationDto { Id = 1, Name = "Andhra Pradesh", Type = "State", Lat = 15.9129, Lon = 79.7400, Capital = "Amaravati", Population = 49500000, Altitude = 1000000 },
                new GeoLocationDto { Id = 2, Name = "Telangana", Type = "State", Lat = 17.8495, Lon = 79.1151, Capital = "Hyderabad", Population = 35000000, Altitude = 800000 },
                new GeoLocationDto { Id = 3, Name = "Karnataka", Type = "State", Lat = 15.3173, Lon = 75.7139, Capital = "Bangalore", Population = 67000000, Altitude = 1000000 },
                new GeoLocationDto { Id = 4, Name = "Tamil Nadu", Type = "State", Lat = 11.1271, Lon = 78.6569, Capital = "Chennai", Population = 72000000, Altitude = 1000000 },
                new GeoLocationDto { Id = 5, Name = "Maharashtra", Type = "State", Lat = 19.7515, Lon = 75.7139, Capital = "Mumbai", Population = 112000000, Altitude = 1200000 },
                new GeoLocationDto { Id = 6, Name = "Kerala", Type = "State", Lat = 10.8505, Lon = 76.2711, Capital = "Thiruvananthapuram", Population = 35000000, Altitude = 800000 }
            };
            return Ok(states);
        }

        return Ok(new List<GeoLocationDto>());
    }

    /// <summary>
    /// Get cities by state
    /// </summary>
    [HttpGet("cities/{stateId}")]
    public ActionResult<List<GeoLocationDto>> GetCities(int stateId)
    {
        // Andhra Pradesh cities
        if (stateId == 1)
        {
            var cities = new List<GeoLocationDto>
            {
                new GeoLocationDto { Id = 1, Name = "Amaravati", Type = "Capital", Lat = 16.5131, Lon = 80.5150, Population = 100000, Altitude = 50000 },
                new GeoLocationDto { Id = 2, Name = "Visakhapatnam", Type = "City", Lat = 17.6868, Lon = 83.2185, Population = 2035000, Altitude = 80000 },
                new GeoLocationDto { Id = 3, Name = "Vijayawada", Type = "City", Lat = 16.5062, Lon = 80.6480, Population = 1500000, Altitude = 80000 },
                new GeoLocationDto { Id = 4, Name = "Guntur", Type = "City", Lat = 16.3067, Lon = 80.4365, Population = 750000, Altitude = 60000 },
                new GeoLocationDto { Id = 5, Name = "Nellore", Type = "City", Lat = 14.4426, Lon = 79.9865, Population = 600000, Altitude = 60000 },
                new GeoLocationDto { Id = 6, Name = "Tirupati", Type = "City", Lat = 13.6288, Lon = 79.4192, Population = 500000, Altitude = 60000 },
                new GeoLocationDto { Id = 7, Name = "Rajahmundry", Type = "City", Lat = 17.0005, Lon = 81.8040, Population = 500000, Altitude = 60000 },
                new GeoLocationDto { Id = 8, Name = "Kakinada", Type = "City", Lat = 16.9891, Lon = 82.2475, Population = 400000, Altitude = 60000 },
                new GeoLocationDto { Id = 9, Name = "Kurnool", Type = "City", Lat = 15.8281, Lon = 78.0373, Population = 480000, Altitude = 60000 },
                new GeoLocationDto { Id = 10, Name = "Kadapa", Type = "City", Lat = 14.4674, Lon = 78.8241, Population = 350000, Altitude = 60000 },
                new GeoLocationDto { Id = 11, Name = "Anantapur", Type = "City", Lat = 14.6819, Lon = 77.6006, Population = 340000, Altitude = 60000 }
            };
            return Ok(cities);
        }

        // Telangana cities
        if (stateId == 2)
        {
            var cities = new List<GeoLocationDto>
            {
                new GeoLocationDto { Id = 12, Name = "Hyderabad", Type = "Capital", Lat = 17.3850, Lon = 78.4867, Population = 10000000, Altitude = 100000 },
                new GeoLocationDto { Id = 13, Name = "Warangal", Type = "City", Lat = 17.9689, Lon = 79.5941, Population = 700000, Altitude = 60000 },
                new GeoLocationDto { Id = 14, Name = "Nizamabad", Type = "City", Lat = 18.6725, Lon = 78.0940, Population = 310000, Altitude = 60000 },
                new GeoLocationDto { Id = 15, Name = "Karimnagar", Type = "City", Lat = 18.4386, Lon = 79.1288, Population = 260000, Altitude = 60000 },
                new GeoLocationDto { Id = 16, Name = "Khammam", Type = "City", Lat = 17.2473, Lon = 80.1514, Population = 200000, Altitude = 60000 }
            };
            return Ok(cities);
        }

        return Ok(new List<GeoLocationDto>());
    }

    /// <summary>
    /// Search locations by name
    /// </summary>
    [HttpGet("search")]
    public ActionResult<List<GeoLocationDto>> SearchLocations([FromQuery] string q, [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
        {
            return Ok(new List<GeoLocationDto>());
        }

        var allLocations = GetAllLocations();
        var results = allLocations
            .Where(l => l.Name.Contains(q, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(l => l.Population)
            .Take(limit)
            .ToList();

        return Ok(results);
    }

    /// <summary>
    /// Get location by coordinates (reverse geocoding)
    /// </summary>
    [HttpGet("reverse")]
    public ActionResult<GeoLocationDto?> ReverseGeocode([FromQuery] double lat, [FromQuery] double lon)
    {
        var allLocations = GetAllLocations();
        
        // Find nearest location within 50km
        var nearest = allLocations
            .Select(l => new { Location = l, Distance = CalculateDistance(lat, lon, l.Lat, l.Lon) })
            .Where(x => x.Distance < 50)
            .OrderBy(x => x.Distance)
            .FirstOrDefault();

        return Ok(nearest?.Location);
    }

    private List<GeoLocationDto> GetAllLocations()
    {
        return new List<GeoLocationDto>
        {
            // Countries
            new GeoLocationDto { Name = "India", Type = "Country", Lat = 20.5937, Lon = 78.9629, Population = 1400000000, Altitude = 5000000 },
            
            // States
            new GeoLocationDto { Name = "Andhra Pradesh", Type = "State", Lat = 15.9129, Lon = 79.7400, Population = 49500000, Altitude = 1000000 },
            new GeoLocationDto { Name = "Telangana", Type = "State", Lat = 17.8495, Lon = 79.1151, Population = 35000000, Altitude = 800000 },
            new GeoLocationDto { Name = "Karnataka", Type = "State", Lat = 15.3173, Lon = 75.7139, Population = 67000000, Altitude = 1000000 },
            new GeoLocationDto { Name = "Tamil Nadu", Type = "State", Lat = 11.1271, Lon = 78.6569, Population = 72000000, Altitude = 1000000 },
            new GeoLocationDto { Name = "Maharashtra", Type = "State", Lat = 19.7515, Lon = 75.7139, Population = 112000000, Altitude = 1200000 },
            
            // AP Cities
            new GeoLocationDto { Name = "Amaravati", Type = "Capital, AP", Lat = 16.5131, Lon = 80.5150, Population = 100000, Altitude = 50000 },
            new GeoLocationDto { Name = "Visakhapatnam", Type = "City, AP", Lat = 17.6868, Lon = 83.2185, Population = 2035000, Altitude = 80000 },
            new GeoLocationDto { Name = "Vijayawada", Type = "City, AP", Lat = 16.5062, Lon = 80.6480, Population = 1500000, Altitude = 80000 },
            new GeoLocationDto { Name = "Guntur", Type = "City, AP", Lat = 16.3067, Lon = 80.4365, Population = 750000, Altitude = 60000 },
            new GeoLocationDto { Name = "Nellore", Type = "City, AP", Lat = 14.4426, Lon = 79.9865, Population = 600000, Altitude = 60000 },
            new GeoLocationDto { Name = "Tirupati", Type = "City, AP", Lat = 13.6288, Lon = 79.4192, Population = 500000, Altitude = 60000 },
            new GeoLocationDto { Name = "Rajahmundry", Type = "City, AP", Lat = 17.0005, Lon = 81.8040, Population = 500000, Altitude = 60000 },
            new GeoLocationDto { Name = "Kakinada", Type = "City, AP", Lat = 16.9891, Lon = 82.2475, Population = 400000, Altitude = 60000 },
            new GeoLocationDto { Name = "Kurnool", Type = "City, AP", Lat = 15.8281, Lon = 78.0373, Population = 480000, Altitude = 60000 },
            new GeoLocationDto { Name = "Kadapa", Type = "City, AP", Lat = 14.4674, Lon = 78.8241, Population = 350000, Altitude = 60000 },
            new GeoLocationDto { Name = "Anantapur", Type = "City, AP", Lat = 14.6819, Lon = 77.6006, Population = 340000, Altitude = 60000 },
            
            // Telangana Cities
            new GeoLocationDto { Name = "Hyderabad", Type = "Capital, TG", Lat = 17.3850, Lon = 78.4867, Population = 10000000, Altitude = 100000 },
            new GeoLocationDto { Name = "Warangal", Type = "City, TG", Lat = 17.9689, Lon = 79.5941, Population = 700000, Altitude = 60000 },
            
            // Major Indian Cities
            new GeoLocationDto { Name = "Mumbai", Type = "City, MH", Lat = 19.0760, Lon = 72.8777, Population = 21000000, Altitude = 150000 },
            new GeoLocationDto { Name = "Delhi", Type = "Capital, India", Lat = 28.6139, Lon = 77.2090, Population = 19000000, Altitude = 150000 },
            new GeoLocationDto { Name = "Bangalore", Type = "City, KA", Lat = 12.9716, Lon = 77.5946, Population = 13000000, Altitude = 120000 },
            new GeoLocationDto { Name = "Chennai", Type = "City, TN", Lat = 13.0827, Lon = 80.2707, Population = 11000000, Altitude = 120000 },
            new GeoLocationDto { Name = "Kolkata", Type = "City, WB", Lat = 22.5726, Lon = 88.3639, Population = 15000000, Altitude = 150000 }
        };
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in km
        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private double ToRad(double deg) => deg * Math.PI / 180;
}

public class GeoLocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string? Capital { get; set; }
    public long Population { get; set; }
    public double Altitude { get; set; } = 500000;
}
