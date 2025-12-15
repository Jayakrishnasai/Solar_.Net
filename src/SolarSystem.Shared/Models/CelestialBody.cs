namespace SolarSystem.Shared.Models;

/// <summary>
/// Represents a celestial body in the solar system (Sun, planets, moons, dwarf planets, asteroids)
/// </summary>
public class CelestialBody
{
    public int Id { get; set; }
    public string ApiId { get; set; } = string.Empty; // ID from external API
    public string Name { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty; // Star, Planet, Dwarf Planet, Moon, Asteroid
    
    // Physical characteristics
    public double Mass { get; set; } // Mass in kg (stored as mantissa)
    public int MassExponent { get; set; } // Mass exponent (e.g., 24 for 10^24)
    public double MeanRadius { get; set; } // in km
    public double EquatorialRadius { get; set; } // in km
    public double PolarRadius { get; set; } // in km
    public double Density { get; set; } // g/cm³
    public double Gravity { get; set; } // m/s²
    public double EscapeSpeed { get; set; } // m/s
    public double MeanTemperature { get; set; } // Kelvin
    public double SiderealRotation { get; set; } // hours
    public double AxialTilt { get; set; } // degrees
    public double Flattening { get; set; }
    
    public bool IsPlanet { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    public string? TextureUrl { get; set; } // For 3D rendering
    
    // Navigation properties
    public Orbit? Orbit { get; set; }
    public Atmosphere? Atmosphere { get; set; }
    public ICollection<PlanetLayer> Layers { get; set; } = new List<PlanetLayer>();
    public ICollection<Moon> Moons { get; set; } = new List<Moon>();
    
    // Parent body (for moons orbiting planets)
    public int? ParentBodyId { get; set; }
    public CelestialBody? ParentBody { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
