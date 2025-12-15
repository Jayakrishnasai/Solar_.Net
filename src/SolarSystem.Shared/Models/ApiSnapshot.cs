namespace SolarSystem.Shared.Models;

/// <summary>
/// Stores raw API response snapshots for caching and offline use
/// </summary>
public class ApiSnapshot
{
    public int Id { get; set; }
    public string Source { get; set; } = string.Empty; // "SolarSystemAPI", "NASA", etc.
    public string Endpoint { get; set; } = string.Empty; // e.g., "/bodies/earth"
    public string RawData { get; set; } = string.Empty; // JSON response
    public DateTime FetchedAt { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}
