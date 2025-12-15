namespace SolarSystem.Shared.DTOs;

/// <summary>
/// DTO for celestial body list view
/// </summary>
public class CelestialBodyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    public double MeanRadius { get; set; }
    public double Mass { get; set; }
    public int MassExponent { get; set; }
    public double Gravity { get; set; }
    public double MeanTemperature { get; set; }
    public bool IsPlanet { get; set; }
    public string? ImageUrl { get; set; }
    public string? TextureUrl { get; set; }
    public int MoonCount { get; set; }
    public OrbitDto? Orbit { get; set; }
}

/// <summary>
/// Detailed celestial body for exploration view
/// </summary>
public class CelestialBodyDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    
    // Physical
    public double Mass { get; set; }
    public int MassExponent { get; set; }
    public double MeanRadius { get; set; }
    public double EquatorialRadius { get; set; }
    public double PolarRadius { get; set; }
    public double Density { get; set; }
    public double Gravity { get; set; }
    public double EscapeSpeed { get; set; }
    public double MeanTemperature { get; set; }
    public double SiderealRotation { get; set; }
    public double AxialTilt { get; set; }
    
    public string? ImageUrl { get; set; }
    public string? TextureUrl { get; set; }
    public string? Description { get; set; }
    
    public OrbitDto? Orbit { get; set; }
    public AtmosphereDto? Atmosphere { get; set; }
    public List<PlanetLayerDto> Layers { get; set; } = new();
    public List<MoonDto> Moons { get; set; } = new();
}

public class OrbitDto
{
    public double SemimajorAxis { get; set; }
    public double Perihelion { get; set; }
    public double Aphelion { get; set; }
    public double Eccentricity { get; set; }
    public double Inclination { get; set; }
    public double OrbitalPeriod { get; set; }
    public double SiderealOrbit { get; set; }
    public double ArgumentOfPerihelion { get; set; }
    public double LongitudeOfAscendingNode { get; set; }
    public double MeanAnomaly { get; set; }
}

public class AtmosphereDto
{
    public bool HasAtmosphere { get; set; }
    public string? Composition { get; set; }
    public double SurfacePressure { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
}

public class PlanetLayerDto
{
    public string LayerName { get; set; } = string.Empty;
    public int LayerOrder { get; set; }
    public double InnerRadius { get; set; }
    public double OuterRadius { get; set; }
    public string? Composition { get; set; }
    public double Temperature { get; set; }
    public string? State { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
}

public class MoonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Radius { get; set; }
    public double OrbitalRadius { get; set; }
    public double OrbitalPeriod { get; set; }
    public string? ImageUrl { get; set; }
}

/// <summary>
/// Simulation state DTO for SignalR synchronization
/// </summary>
public class SimulationStateDto
{
    public DateTime SimulatedTime { get; set; }
    public double TimeMultiplier { get; set; }
    public bool IsPaused { get; set; }
    public Dictionary<int, PlanetPositionDto> Positions { get; set; } = new();
}

public class PlanetPositionDto
{
    public int Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double Rotation { get; set; } // Current rotation angle
}
