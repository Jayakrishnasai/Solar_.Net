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
            
            // Map column names to PostgreSQL lowercase
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ApiId).HasColumnName("external_id");
            entity.Property(e => e.EnglishName).HasColumnName("english_name");
            entity.Property(e => e.BodyType).HasColumnName("body_type");
            entity.Property(e => e.IsPlanet).HasColumnName("is_planet");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            
            // Ignore columns that don't exist in DB
            entity.Ignore(e => e.Mass);
            entity.Ignore(e => e.MassExponent);
            entity.Ignore(e => e.MeanRadius);
            entity.Ignore(e => e.EquatorialRadius);
            entity.Ignore(e => e.PolarRadius);
            entity.Ignore(e => e.Density);
            entity.Ignore(e => e.Gravity);
            entity.Ignore(e => e.EscapeSpeed);
            entity.Ignore(e => e.MeanTemperature);
            entity.Ignore(e => e.SiderealRotation);
            entity.Ignore(e => e.AxialTilt);
            entity.Ignore(e => e.Flattening);
            entity.Ignore(e => e.ImageUrl);
            entity.Ignore(e => e.TextureUrl);
            entity.Ignore(e => e.UpdatedAt);
            entity.Ignore(e => e.ParentBodyId);
            entity.Ignore(e => e.ParentBody);
            entity.Ignore(e => e.Layers);
            entity.Ignore(e => e.Moons);
            entity.Ignore(e => e.Orbit);
            entity.Ignore(e => e.Atmosphere);
            
            entity.HasIndex(e => e.ApiId).IsUnique();
            entity.HasIndex(e => e.EnglishName);
            entity.HasIndex(e => e.BodyType);
        });

        // Orbit configuration
        modelBuilder.Entity<Orbit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CelestialBodyId).HasColumnName("celestial_body_id");
            entity.Property(e => e.SemimajorAxis).HasColumnName("semi_major_axis_km");
            entity.Property(e => e.Eccentricity).HasColumnName("eccentricity");
            entity.Property(e => e.Inclination).HasColumnName("inclination");
            entity.Property(e => e.OrbitalPeriod).HasColumnName("orbital_period_days");
            // Ignore unmapped properties
            entity.Ignore(e => e.Perihelion);
            entity.Ignore(e => e.Aphelion);
            entity.Ignore(e => e.SiderealOrbit);
            entity.Ignore(e => e.MeanAnomaly);
            entity.Ignore(e => e.ArgumentOfPerihelion);
            entity.Ignore(e => e.LongitudeOfAscendingNode);
            entity.Ignore(e => e.CelestialBody);
        });

        // PlanetLayer configuration
        modelBuilder.Entity<PlanetLayer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CelestialBodyId).HasColumnName("planet_id");
            entity.Property(e => e.LayerOrder).HasColumnName("layer_order");
            entity.Property(e => e.LayerName).HasColumnName("layer_name");
            entity.Property(e => e.Composition).HasColumnName("composition");
            entity.Property(e => e.Temperature).HasColumnName("avg_temperature");
            // Ignore unmapped
            entity.Ignore(e => e.InnerRadius);
            entity.Ignore(e => e.OuterRadius);
            entity.Ignore(e => e.State);
            entity.Ignore(e => e.Color);
            entity.Ignore(e => e.Description);
            entity.Ignore(e => e.CelestialBody);
        });

        // Atmosphere configuration
        modelBuilder.Entity<Atmosphere>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CelestialBodyId).HasColumnName("planet_id");
            entity.Property(e => e.Composition).HasColumnName("primary_gases");
            entity.Property(e => e.SurfacePressure).HasColumnName("pressure_bar");
            // Ignore unmapped
            entity.Ignore(e => e.HasAtmosphere);
            entity.Ignore(e => e.Color);
            entity.Ignore(e => e.Description);
            entity.Ignore(e => e.CelestialBody);
        });

        // Moon configuration
        modelBuilder.Entity<Moon>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ParentBodyId).HasColumnName("celestial_body_id");
            entity.Property(e => e.Radius).HasColumnName("mean_radius_km");
            entity.Property(e => e.OrbitalPeriod).HasColumnName("orbital_period_days");
            // Map planet_id to separate column
            entity.Ignore(e => e.ApiId);
            entity.Ignore(e => e.Name);
            entity.Ignore(e => e.Mass);
            entity.Ignore(e => e.MassExponent);
            entity.Ignore(e => e.OrbitalRadius);
            entity.Ignore(e => e.Eccentricity);
            entity.Ignore(e => e.Inclination);
            entity.Ignore(e => e.Description);
            entity.Ignore(e => e.ImageUrl);
            entity.Ignore(e => e.ParentBody);
        });

        // ApiSnapshot configuration
        modelBuilder.Entity<ApiSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Source).HasColumnName("source_name");
            entity.Property(e => e.Endpoint).HasColumnName("endpoint");
            entity.Property(e => e.RawData).HasColumnName("raw_json");
            entity.Property(e => e.FetchedAt).HasColumnName("fetched_at");
            entity.Ignore(e => e.IsValid);
            entity.Ignore(e => e.ErrorMessage);
        });

        // SimulationState configuration
        modelBuilder.Entity<SimulationState>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
