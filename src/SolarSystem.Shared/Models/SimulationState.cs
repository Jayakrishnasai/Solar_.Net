namespace SolarSystem.Shared.Models;

/// <summary>
/// Current simulation state for synchronization across clients
/// </summary>
public class SimulationState
{
    public int Id { get; set; }
    public DateTime SimulatedTime { get; set; } // The simulated date/time in the solar system
    public double TimeMultiplier { get; set; } = 1.0; // 1 = real-time, >1 = faster
    public bool IsPaused { get; set; }
    public DateTime LastUpdated { get; set; }
    public string? SessionId { get; set; } // For multi-session support
}
