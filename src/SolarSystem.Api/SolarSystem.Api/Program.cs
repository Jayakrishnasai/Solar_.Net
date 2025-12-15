using Microsoft.EntityFrameworkCore;
using SolarSystem.Api.Data;
using SolarSystem.Api.Hubs;
using SolarSystem.Api.Client.Pages;
using SolarSystem.Api.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Add API controllers
builder.Services.AddControllers();

// Add SignalR
builder.Services.AddSignalR();

// Add Database (PostgreSQL for production, SQLite for development)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SolarSystemDbContext>(options =>
{
    if (builder.Environment.IsProduction() && connectionString?.Contains("Host=") == true)
    {
        // PostgreSQL (Supabase)
        options.UseNpgsql(connectionString);
    }
    else
    {
        // SQLite for local development
        options.UseSqlite(connectionString ?? "Data Source=solarsystem.db");
    }
});

// Add HttpClient for external APIs
builder.Services.AddHttpClient("SolarSystemApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["SolarSystemApi:BaseUrl"] ?? "https://api.le-systeme-solaire.net/rest/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("NasaApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["NasaApi:BaseUrl"] ?? "https://api.nasa.gov/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add API sync services
builder.Services.AddScoped<SolarSystem.Api.Services.SolarSystemApiService>();
builder.Services.AddScoped<SolarSystem.Api.Services.NasaApiService>();
builder.Services.AddHostedService<SolarSystem.Api.Services.DataSyncBackgroundService>();

var app = builder.Build();

// Initialize database and seed data
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<SolarSystemDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Attempting database connection...");
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        logger.LogInformation("Database connected successfully!");
        
        // Seed fallback data
        await DataSeeder.SeedAsync(context);
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Database initialization failed. App will continue without database.");
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAntiforgery();

app.MapStaticAssets();

// Map API controllers
app.MapControllers();

// Map SignalR hub
app.MapHub<SimulationHub>("/hubs/simulation");

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SolarSystem.Api.Client._Imports).Assembly);

app.Run();
