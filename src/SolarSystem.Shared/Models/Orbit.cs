namespace SolarSystem.Shared.Models;

/// <summary>
/// Orbital parameters for a celestial body
/// </summary>
public class Orbit
{
    public int Id { get; set; }
    public int CelestialBodyId { get; set; }
    
    // Orbital elements
    public double SemimajorAxis { get; set; } // in km
    public double Perihelion { get; set; } // in km
    public double Aphelion { get; set; } // in km
    public double Eccentricity { get; set; }
    public double Inclination { get; set; } // degrees
    public double OrbitalPeriod { get; set; } // Earth days
    public double SiderealOrbit { get; set; } // Earth days
    public double ArgumentOfPerihelion { get; set; } // degrees (ω)
    public double LongitudeOfAscendingNode { get; set; } // degrees (Ω)
    public double MeanAnomaly { get; set; } // degrees at epoch
    
    // Computed/Reference
    public double MeanOrbitalSpeed { get; set; } // km/s
    
    // Navigation
    public CelestialBody? CelestialBody { get; set; }
}
