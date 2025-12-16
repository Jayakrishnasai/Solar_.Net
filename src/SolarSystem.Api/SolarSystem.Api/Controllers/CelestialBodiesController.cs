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
    /// Get all celestial bodies with basic data
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CelestialBodyDto>>> GetAll()
    {
        var bodies = await _context.CelestialBodies
            .OrderBy(b => b.BodyType == "Star" ? 0 : 1)
            .ThenBy(b => b.Id)
            .Select(b => new CelestialBodyDto
            {
                Id = b.Id,
                Name = b.Name,
                EnglishName = b.EnglishName,
                BodyType = b.BodyType,
                IsPlanet = b.IsPlanet
            })
            .ToListAsync();

        return Ok(bodies);
    }

    /// <summary>
    /// Get detailed info for a single celestial body
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CelestialBodyDetailDto>> GetById(int id)
    {
        var body = await _context.CelestialBodies
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
            Description = body.Description
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
            .FirstOrDefaultAsync(b => b.EnglishName.ToLower() == name.ToLower());

        if (body == null)
        {
            return NotFound();
        }

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
            .OrderBy(b => b.BodyType == "Star" ? 0 : 1)
            .ThenBy(b => b.Id)
            .Select(b => new CelestialBodyDto
            {
                Id = b.Id,
                Name = b.Name,
                EnglishName = b.EnglishName,
                BodyType = b.BodyType,
                IsPlanet = b.IsPlanet
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
