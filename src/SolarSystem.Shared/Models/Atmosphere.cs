namespace SolarSystem.Shared.Models;

/// <summary>
/// Atmospheric properties of a celestial body
/// </summary>
public class Atmosphere
{
    public int Id { get; set; }
    public int CelestialBodyId { get; set; }
    
    public bool HasAtmosphere { get; set; }
    public string? Composition { get; set; } // e.g., "78% Nitrogen, 21% Oxygen"
    public double SurfacePressure { get; set; } // in atmospheres (atm)
    public double ScaleHeight { get; set; } // km
    public double MinTemperature { get; set; } // Kelvin
    public double MaxTemperature { get; set; } // Kelvin
    public string? Color { get; set; } // Dominant color for visualization
    public string? Description { get; set; }
    
    public CelestialBody? CelestialBody { get; set; }
}
