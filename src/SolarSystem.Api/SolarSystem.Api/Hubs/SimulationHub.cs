using Microsoft.AspNetCore.SignalR;
using SolarSystem.Shared.DTOs;

namespace SolarSystem.Api.Hubs;

/// <summary>
/// SignalR Hub for real-time simulation synchronization
/// </summary>
public class SimulationHub : Hub
{
    private static SimulationStateDto _currentState = new()
    {
        SimulatedTime = DateTime.UtcNow,
        TimeMultiplier = 1.0,
        IsPaused = true,
        Positions = new Dictionary<int, PlanetPositionDto>()
    };

    private static readonly object _lock = new();

    /// <summary>
    /// Called when a client connects - sends current state
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("ReceiveSimulationState", _currentState);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Update time multiplier (simulation speed)
    /// </summary>
    public async Task UpdateTimeMultiplier(double multiplier)
    {
        lock (_lock)
        {
            _currentState.TimeMultiplier = Math.Clamp(multiplier, 0.1, 10000);
        }
        await Clients.All.SendAsync("TimeMultiplierChanged", _currentState.TimeMultiplier);
    }

    /// <summary>
    /// Toggle simulation pause state
    /// </summary>
    public async Task TogglePause(bool isPaused)
    {
        lock (_lock)
        {
            _currentState.IsPaused = isPaused;
        }
        await Clients.All.SendAsync("PauseStateChanged", _currentState.IsPaused);
    }

    /// <summary>
    /// Set simulated time to a specific date
    /// </summary>
    public async Task SetSimulatedTime(DateTime time)
    {
        lock (_lock)
        {
            _currentState.SimulatedTime = time;
        }
        await Clients.All.SendAsync("SimulatedTimeChanged", _currentState.SimulatedTime);
    }

    /// <summary>
    /// Broadcast planet positions to all clients (called by server periodically)
    /// </summary>
    public async Task BroadcastPositions(Dictionary<int, PlanetPositionDto> positions)
    {
        lock (_lock)
        {
            _currentState.Positions = positions;
        }
        await Clients.All.SendAsync("PositionsUpdated", positions);
    }

    /// <summary>
    /// Client requests full state resync
    /// </summary>
    public async Task RequestStateSync()
    {
        await Clients.Caller.SendAsync("ReceiveSimulationState", _currentState);
    }

    /// <summary>
    /// Focus on a specific planet (broadcast to all)
    /// </summary>
    public async Task FocusPlanet(int planetId, string planetName)
    {
        await Clients.All.SendAsync("PlanetFocused", planetId, planetName);
    }

    /// <summary>
    /// Get current state
    /// </summary>
    public SimulationStateDto GetCurrentState()
    {
        return _currentState;
    }
}
