using Microsoft.EntityFrameworkCore;
using SolarSystem.Shared.Models;

namespace SolarSystem.Api.Data;

public class SolarSystemDbContext : DbContext
{
    public SolarSystemDbContext(DbContextOptions<SolarSystemDbContext> options) 
        : base(options)
    {
    }

    public DbSet<CelestialBody> CelestialBodies { get; set; }
    public DbSet<Orbit> Orbits { get; set; }
    public DbSet<PlanetLayer> PlanetLayers { get; set; }
    public DbSet<Atmosphere> Atmospheres { get; set; }
    public DbSet<Moon> Moons { get; set; }
    public DbSet<ApiSnapshot> ApiSnapshots { get; set; }
    public DbSet<SimulationState> SimulationStates { get; set; }
    public DbSet<ContactSubmission> ContactSubmissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map to PostgreSQL snake_case table names
        modelBuilder.Entity<CelestialBody>().ToTable("celestial_bodies");
        modelBuilder.Entity<Orbit>().ToTable("orbits");
        modelBuilder.Entity<PlanetLayer>().ToTable("planet_layers");
        modelBuilder.Entity<Atmosphere>().ToTable("atmospheres");
        modelBuilder.Entity<Moon>().ToTable("moons");
        modelBuilder.Entity<ApiSnapshot>().ToTable("api_snapshots");
        modelBuilder.Entity<SimulationState>().ToTable("SimulationStates");
        modelBuilder.Entity<ContactSubmission>().ToTable("ContactSubmissions");

        // CelestialBody configuration
        modelBuilder.Entity<CelestialBody>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ApiId).IsUnique();
            entity.HasIndex(e => e.EnglishName);
            entity.HasIndex(e => e.BodyType);

            entity.HasOne(e => e.Orbit)
                .WithOne(o => o.CelestialBody)
                .HasForeignKey<Orbit>(o => o.CelestialBodyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Atmosphere)
                .WithOne(a => a.CelestialBody)
                .HasForeignKey<Atmosphere>(a => a.CelestialBodyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Layers)
                .WithOne(l => l.CelestialBody)
                .HasForeignKey(l => l.CelestialBodyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Moons)
                .WithOne(m => m.ParentBody)
                .HasForeignKey(m => m.ParentBodyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Self-referencing for moons orbiting planets
            entity.HasOne(e => e.ParentBody)
                .WithMany()
                .HasForeignKey(e => e.ParentBodyId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Orbit configuration
        modelBuilder.Entity<Orbit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CelestialBodyId).IsUnique();
        });

        // PlanetLayer configuration
        modelBuilder.Entity<PlanetLayer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CelestialBodyId, e.LayerOrder });
        });

        // Atmosphere configuration
        modelBuilder.Entity<Atmosphere>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CelestialBodyId).IsUnique();
        });

        // Moon configuration
        modelBuilder.Entity<Moon>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ApiId);
            entity.HasIndex(e => e.ParentBodyId);
        });

        // ApiSnapshot configuration
        modelBuilder.Entity<ApiSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Source, e.Endpoint });
            entity.HasIndex(e => e.FetchedAt);
        });

        // SimulationState configuration
        modelBuilder.Entity<SimulationState>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
