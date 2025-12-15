namespace SolarSystem.Shared.Models;

/// <summary>
/// Moon (natural satellite) of a planet
/// </summary>
public class Moon
{
    public int Id { get; set; }
    public int ParentBodyId { get; set; } // The planet this moon orbits
    public string ApiId { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    public double Radius { get; set; } // km
    public double Mass { get; set; } // kg (mantissa)
    public int MassExponent { get; set; }
    public double OrbitalRadius { get; set; } // km from parent
    public double OrbitalPeriod { get; set; } // Earth days
    public double Eccentricity { get; set; }
    public double Inclination { get; set; } // degrees
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }
    
    public CelestialBody? ParentBody { get; set; }
}
