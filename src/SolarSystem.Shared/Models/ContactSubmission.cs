namespace SolarSystem.Shared.Models;

/// <summary>
/// Contact form submission stored in database
/// </summary>
public class ContactSubmission
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Message { get; set; } = "";
    public string? Category { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
    public bool IsResolved { get; set; } = false;
    public string? AdminNotes { get; set; }
}
