namespace SolarSystem.Shared.Models;

/// <summary>
/// Internal layer of a planet (core, mantle, crust)
/// </summary>
public class PlanetLayer
{
    public int Id { get; set; }
    public int CelestialBodyId { get; set; }
    
    public string LayerName { get; set; } = string.Empty; // Inner Core, Outer Core, Mantle, Crust
    public int LayerOrder { get; set; } // 1 = innermost
    public double InnerRadius { get; set; } // km
    public double OuterRadius { get; set; } // km
    public string? Composition { get; set; } // e.g., "Iron, Nickel"
    public double Temperature { get; set; } // Kelvin
    public string? State { get; set; } // Solid, Liquid, Gas
    public string? Color { get; set; } // For visualization
    public string? Description { get; set; }
    
    public CelestialBody? CelestialBody { get; set; }
}
