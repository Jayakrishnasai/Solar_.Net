using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarSystem.Api.Data;

namespace SolarSystem.Api.Controllers;

/// <summary>
/// Database verification and diagnostics endpoint
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly SolarSystemDbContext _context;
    private readonly ILogger<DatabaseController> _logger;

    public DatabaseController(SolarSystemDbContext context, ILogger<DatabaseController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get complete database health status
    /// </summary>
    [HttpGet("health")]
    public async Task<ActionResult<DatabaseHealthReport>> GetHealthReport()
    {
        var report = new DatabaseHealthReport();

        try
        {
            // 1. Connection Check
            var canConnect = await _context.Database.CanConnectAsync();
            report.IsConnected = canConnect;
            report.ConnectionStatus = canConnect ? "✅ Connected" : "❌ Connection Failed";

            if (!canConnect)
            {
                report.OverallStatus = "❌ Database Unreachable";
                return Ok(report);
            }

            // 2. Database Info
            report.DatabaseName = _context.Database.GetDbConnection().Database;
            report.Provider = _context.Database.ProviderName ?? "Unknown";
            report.ConnectionString = MaskConnectionString(_context.Database.GetConnectionString() ?? "");

            // 3. Migration Status
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
            var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync();
            report.AppliedMigrations = appliedMigrations.ToList();
            report.PendingMigrations = pendingMigrations.ToList();
            report.MigrationStatus = pendingMigrations.Any() 
                ? $"⚠ {pendingMigrations.Count()} Pending" 
                : "✅ All Applied";

            // 4. Table Record Counts
            report.TableCounts = new TableCountsDto
            {
                CelestialBodies = await _context.CelestialBodies.CountAsync(),
                Planets = await _context.CelestialBodies.CountAsync(c => c.IsPlanet),
                Stars = await _context.CelestialBodies.CountAsync(c => c.BodyType == "Star"),
                Orbits = await _context.Orbits.CountAsync(),
                PlanetLayers = await _context.PlanetLayers.CountAsync(),
                Atmospheres = await _context.Atmospheres.CountAsync(),
                Moons = await _context.Moons.CountAsync(),
                ApiSnapshots = await _context.ApiSnapshots.CountAsync(),
                SimulationStates = await _context.SimulationStates.CountAsync()
            };

            // Check for empty tables
            report.EmptyTables = new List<string>();
            if (report.TableCounts.CelestialBodies == 0) report.EmptyTables.Add("CelestialBodies");
            if (report.TableCounts.Orbits == 0) report.EmptyTables.Add("Orbits");
            if (report.TableCounts.PlanetLayers == 0) report.EmptyTables.Add("PlanetLayers");

            report.DataStatus = report.EmptyTables.Any() 
                ? $"⚠ {report.EmptyTables.Count} Empty Tables" 
                : "✅ All Tables Have Data";

            // 5. Sample Planets
            report.SamplePlanets = await _context.CelestialBodies
                .Where(c => c.IsPlanet || c.BodyType == "Star")
                .OrderBy(c => c.Id)
                .Take(5)
                .Select(c => new PlanetSampleDto
                {
                    Id = c.Id,
                    Name = c.EnglishName,
                    BodyType = c.BodyType
                })
                .ToListAsync();

            // 6. API Sync Info
            var lastSnapshot = await _context.ApiSnapshots
                .OrderByDescending(s => s.FetchedAt)
                .FirstOrDefaultAsync();

            report.ApiSyncInfo = new ApiSyncInfoDto
            {
                LastSyncTime = lastSnapshot?.FetchedAt,
                DataSource = "Le Système Solaire OpenData API",
                TotalSnapshots = await _context.ApiSnapshots.CountAsync(),
                SuccessfulSyncs = await _context.ApiSnapshots.CountAsync(s => s.IsValid),
                FailedSyncs = await _context.ApiSnapshots.CountAsync(s => !s.IsValid)
            };

            report.ApiStatus = lastSnapshot != null 
                ? "✅ API Data Available" 
                : "⚠ No API Data Yet";

            // 7. Foreign Key Validation
            report.ForeignKeyChecks = new ForeignKeyChecksDto
            {
                OrphansInOrbits = await _context.Orbits
                    .CountAsync(o => !_context.CelestialBodies.Any(c => c.Id == o.CelestialBodyId)),
                OrphansInLayers = await _context.PlanetLayers
                    .CountAsync(l => !_context.CelestialBodies.Any(c => c.Id == l.CelestialBodyId)),
                OrphansInMoons = await _context.Moons
                    .CountAsync(m => !_context.CelestialBodies.Any(c => c.Id == m.ParentBodyId))
            };

            var hasOrphans = report.ForeignKeyChecks.OrphansInOrbits > 0 ||
                             report.ForeignKeyChecks.OrphansInLayers > 0 ||
                             report.ForeignKeyChecks.OrphansInMoons > 0;

            report.IntegrityStatus = hasOrphans 
                ? "⚠ Orphan Records Found" 
                : "✅ All References Valid";

            // 8. Overall Status
            var issues = new List<string>();
            if (!canConnect) issues.Add("Connection");
            if (pendingMigrations.Any()) issues.Add("Migrations");
            if (report.EmptyTables.Any()) issues.Add("Empty Tables");
            if (hasOrphans) issues.Add("Orphan Records");

            report.OverallStatus = issues.Count switch
            {
                0 => "✅ All Systems Operational",
                1 => $"⚠ Minor Issue: {issues[0]}",
                _ => $"❌ {issues.Count} Issues Found"
            };

            report.CheckedAt = DateTime.UtcNow;
            report.IsHealthy = issues.Count == 0;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking database health");
            report.IsConnected = false;
            report.ConnectionStatus = "❌ Error";
            report.OverallStatus = $"❌ Error: {ex.Message}";
            report.ErrorMessage = ex.Message;
        }

        return Ok(report);
    }

    private string MaskConnectionString(string connectionString)
    {
        // Mask sensitive parts of connection string
        if (string.IsNullOrEmpty(connectionString)) return "N/A";
        if (connectionString.Contains("Password="))
        {
            return System.Text.RegularExpressions.Regex.Replace(
                connectionString, @"Password=[^;]+", "Password=***");
        }
        return connectionString;
    }
}

// DTOs for the health report
public class DatabaseHealthReport
{
    public bool IsHealthy { get; set; }
    public bool IsConnected { get; set; }
    public string ConnectionStatus { get; set; } = "";
    public string OverallStatus { get; set; } = "";
    public string? ErrorMessage { get; set; }
    public DateTime CheckedAt { get; set; }
    
    // Database Info
    public string DatabaseName { get; set; } = "";
    public string Provider { get; set; } = "";
    public string ConnectionString { get; set; } = "";
    
    // Migration Info
    public string MigrationStatus { get; set; } = "";
    public List<string> AppliedMigrations { get; set; } = new();
    public List<string> PendingMigrations { get; set; } = new();
    
    // Data Counts
    public TableCountsDto TableCounts { get; set; } = new();
    public string DataStatus { get; set; } = "";
    public List<string> EmptyTables { get; set; } = new();
    
    // Sample Data
    public List<PlanetSampleDto> SamplePlanets { get; set; } = new();
    
    // API Sync
    public ApiSyncInfoDto ApiSyncInfo { get; set; } = new();
    public string ApiStatus { get; set; } = "";
    
    // Integrity
    public ForeignKeyChecksDto ForeignKeyChecks { get; set; } = new();
    public string IntegrityStatus { get; set; } = "";
}

public class TableCountsDto
{
    public int CelestialBodies { get; set; }
    public int Planets { get; set; }
    public int Stars { get; set; }
    public int Orbits { get; set; }
    public int PlanetLayers { get; set; }
    public int Atmospheres { get; set; }
    public int Moons { get; set; }
    public int ApiSnapshots { get; set; }
    public int SimulationStates { get; set; }
}

public class PlanetSampleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string BodyType { get; set; } = "";
    public double MeanRadius { get; set; }
    public bool HasOrbit { get; set; }
    public double OrbitalPeriod { get; set; }
    public double SemimajorAxis { get; set; }
    public int LayerCount { get; set; }
    public int MoonCount { get; set; }
}

public class ApiSyncInfoDto
{
    public DateTime? LastSyncTime { get; set; }
    public string DataSource { get; set; } = "";
    public int TotalSnapshots { get; set; }
    public int SuccessfulSyncs { get; set; }
    public int FailedSyncs { get; set; }
}

public class ForeignKeyChecksDto
{
    public int OrphansInOrbits { get; set; }
    public int OrphansInLayers { get; set; }
    public int OrphansInMoons { get; set; }
}
