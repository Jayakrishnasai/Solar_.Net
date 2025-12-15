using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarSystem.Api.Data;
using SolarSystem.Shared.DTOs;
using SolarSystem.Shared.Models;

namespace SolarSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CelestialBodiesController : ControllerBase
{
    private readonly SolarSystemDbContext _context;
    private readonly ILogger<CelestialBodiesController> _logger;

    public CelestialBodiesController(SolarSystemDbContext context, ILogger<CelestialBodiesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all celestial bodies with basic orbit data
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CelestialBodyDto>>> GetAll()
    {
        var bodies = await _context.CelestialBodies
            .Include(b => b.Orbit)
            .Include(b => b.Moons)
            .OrderBy(b => b.BodyType == "Star" ? 0 : 1)
            .ThenBy(b => b.Orbit != null ? b.Orbit.SemimajorAxis : 0)
            .Select(b => new CelestialBodyDto
            {
                Id = b.Id,
                Name = b.Name,
                EnglishName = b.EnglishName,
                BodyType = b.BodyType,
                MeanRadius = b.MeanRadius,
                Mass = b.Mass,
                MassExponent = b.MassExponent,
                Gravity = b.Gravity,
                MeanTemperature = b.MeanTemperature,
                IsPlanet = b.IsPlanet,
                ImageUrl = b.ImageUrl,
                TextureUrl = b.TextureUrl,
                MoonCount = b.Moons.Count,
                Orbit = b.Orbit != null ? new OrbitDto
                {
                    SemimajorAxis = b.Orbit.SemimajorAxis,
                    Perihelion = b.Orbit.Perihelion,
                    Aphelion = b.Orbit.Aphelion,
                    Eccentricity = b.Orbit.Eccentricity,
                    Inclination = b.Orbit.Inclination,
                    OrbitalPeriod = b.Orbit.OrbitalPeriod,
                    SiderealOrbit = b.Orbit.SiderealOrbit,
                    ArgumentOfPerihelion = b.Orbit.ArgumentOfPerihelion,
                    LongitudeOfAscendingNode = b.Orbit.LongitudeOfAscendingNode,
                    MeanAnomaly = b.Orbit.MeanAnomaly
                } : null
            })
            .ToListAsync();

        return Ok(bodies);
    }

    /// <summary>
    /// Get detailed info for a single celestial body including layers and moons
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CelestialBodyDetailDto>> GetById(int id)
    {
        var body = await _context.CelestialBodies
            .Include(b => b.Orbit)
            .Include(b => b.Atmosphere)
            .Include(b => b.Layers.OrderBy(l => l.LayerOrder))
            .Include(b => b.Moons)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (body == null)
        {
            return NotFound();
        }

        var dto = new CelestialBodyDetailDto
        {
            Id = body.Id,
            Name = body.Name,
            EnglishName = body.EnglishName,
            BodyType = body.BodyType,
            Mass = body.Mass,
            MassExponent = body.MassExponent,
            MeanRadius = body.MeanRadius,
            EquatorialRadius = body.EquatorialRadius,
            PolarRadius = body.PolarRadius,
            Density = body.Density,
            Gravity = body.Gravity,
            EscapeSpeed = body.EscapeSpeed,
            MeanTemperature = body.MeanTemperature,
            SiderealRotation = body.SiderealRotation,
            AxialTilt = body.AxialTilt,
            ImageUrl = body.ImageUrl,
            TextureUrl = body.TextureUrl,
            Description = body.Description,
            Orbit = body.Orbit != null ? new OrbitDto
            {
                SemimajorAxis = body.Orbit.SemimajorAxis,
                Perihelion = body.Orbit.Perihelion,
                Aphelion = body.Orbit.Aphelion,
                Eccentricity = body.Orbit.Eccentricity,
                Inclination = body.Orbit.Inclination,
                OrbitalPeriod = body.Orbit.OrbitalPeriod,
                SiderealOrbit = body.Orbit.SiderealOrbit,
                ArgumentOfPerihelion = body.Orbit.ArgumentOfPerihelion,
                LongitudeOfAscendingNode = body.Orbit.LongitudeOfAscendingNode,
                MeanAnomaly = body.Orbit.MeanAnomaly
            } : null,
            Atmosphere = body.Atmosphere != null ? new AtmosphereDto
            {
                HasAtmosphere = body.Atmosphere.HasAtmosphere,
                Composition = body.Atmosphere.Composition,
                SurfacePressure = body.Atmosphere.SurfacePressure,
                Color = body.Atmosphere.Color,
                Description = body.Atmosphere.Description
            } : null,
            Layers = body.Layers.Select(l => new PlanetLayerDto
            {
                LayerName = l.LayerName,
                LayerOrder = l.LayerOrder,
                InnerRadius = l.InnerRadius,
                OuterRadius = l.OuterRadius,
                Composition = l.Composition,
                Temperature = l.Temperature,
                State = l.State,
                Color = l.Color,
                Description = l.Description
            }).ToList(),
            Moons = body.Moons.Select(m => new MoonDto
            {
                Id = m.Id,
                Name = m.Name,
                Radius = m.Radius,
                OrbitalRadius = m.OrbitalRadius,
                OrbitalPeriod = m.OrbitalPeriod,
                ImageUrl = m.ImageUrl
            }).ToList()
        };

        return Ok(dto);
    }

    /// <summary>
    /// Get celestial body by name
    /// </summary>
    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<CelestialBodyDetailDto>> GetByName(string name)
    {
        var body = await _context.CelestialBodies
            .Include(b => b.Orbit)
            .Include(b => b.Atmosphere)
            .Include(b => b.Layers.OrderBy(l => l.LayerOrder))
            .Include(b => b.Moons)
            .FirstOrDefaultAsync(b => b.EnglishName.ToLower() == name.ToLower());

        if (body == null)
        {
            return NotFound();
        }

        // Reuse the same mapping logic
        return await GetById(body.Id);
    }

    /// <summary>
    /// Get only planets (for main visualization)
    /// </summary>
    [HttpGet("planets")]
    public async Task<ActionResult<IEnumerable<CelestialBodyDto>>> GetPlanets()
    {
        var planets = await _context.CelestialBodies
            .Where(b => b.IsPlanet || b.BodyType == "Star")
            .Include(b => b.Orbit)
            .Include(b => b.Moons)
            .OrderBy(b => b.BodyType == "Star" ? 0 : 1)
            .ThenBy(b => b.Orbit != null ? b.Orbit.SemimajorAxis : 0)
            .Select(b => new CelestialBodyDto
            {
                Id = b.Id,
                Name = b.Name,
                EnglishName = b.EnglishName,
                BodyType = b.BodyType,
                MeanRadius = b.MeanRadius,
                Mass = b.Mass,
                MassExponent = b.MassExponent,
                Gravity = b.Gravity,
                MeanTemperature = b.MeanTemperature,
                IsPlanet = b.IsPlanet,
                ImageUrl = b.ImageUrl,
                TextureUrl = b.TextureUrl,
                MoonCount = b.Moons.Count,
                Orbit = b.Orbit != null ? new OrbitDto
                {
                    SemimajorAxis = b.Orbit.SemimajorAxis,
                    Perihelion = b.Orbit.Perihelion,
                    Aphelion = b.Orbit.Aphelion,
                    Eccentricity = b.Orbit.Eccentricity,
                    Inclination = b.Orbit.Inclination,
                    OrbitalPeriod = b.Orbit.OrbitalPeriod,
                    SiderealOrbit = b.Orbit.SiderealOrbit,
                    MeanAnomaly = b.Orbit.MeanAnomaly
                } : null
            })
            .ToListAsync();

        return Ok(planets);
    }

    /// <summary>
    /// Get layers for a specific celestial body
    /// </summary>
    [HttpGet("{id}/layers")]
    public async Task<ActionResult<IEnumerable<PlanetLayerDto>>> GetLayers(int id)
    {
        var layers = await _context.PlanetLayers
            .Where(l => l.CelestialBodyId == id)
            .OrderBy(l => l.LayerOrder)
            .Select(l => new PlanetLayerDto
            {
                LayerName = l.LayerName,
                LayerOrder = l.LayerOrder,
                InnerRadius = l.InnerRadius,
                OuterRadius = l.OuterRadius,
                Composition = l.Composition,
                Temperature = l.Temperature,
                State = l.State,
                Color = l.Color,
                Description = l.Description
            })
            .ToListAsync();

        return Ok(layers);
    }

    /// <summary>
    /// Get moons for a specific celestial body
    /// </summary>
    [HttpGet("{id}/moons")]
    public async Task<ActionResult<IEnumerable<MoonDto>>> GetMoons(int id)
    {
        var moons = await _context.Moons
            .Where(m => m.ParentBodyId == id)
            .Select(m => new MoonDto
            {
                Id = m.Id,
                Name = m.Name,
                Radius = m.Radius,
                OrbitalRadius = m.OrbitalRadius,
                OrbitalPeriod = m.OrbitalPeriod,
                ImageUrl = m.ImageUrl
            })
            .ToListAsync();

        return Ok(moons);
    }
}
