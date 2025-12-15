using SolarSystem.Api.Services;

namespace SolarSystem.Api.Services;

/// <summary>
/// Background service that periodically syncs data from the Solar System API
/// </summary>
public class DataSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DataSyncBackgroundService> _logger;
    private readonly IConfiguration _configuration;

    public DataSyncBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<DataSyncBackgroundService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Data Sync Background Service starting...");
        
        // Initial delay before first sync
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var enabled = _configuration.GetValue<bool>("SolarSystemApi:EnableAutoRefresh");
                
                if (enabled)
                {
                    _logger.LogInformation("Running scheduled API sync...");
                    
                    using var scope = _scopeFactory.CreateScope();
                    var apiService = scope.ServiceProvider.GetRequiredService<SolarSystemApiService>();
                    
                    await apiService.SyncToDatabase();
                    
                    _logger.LogInformation("Scheduled sync completed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background sync");
            }

            // Wait for next sync interval
            var hours = _configuration.GetValue<int>("SolarSystemApi:RefreshIntervalHours", 24);
            await Task.Delay(TimeSpan.FromHours(hours), stoppingToken);
        }
    }
}
