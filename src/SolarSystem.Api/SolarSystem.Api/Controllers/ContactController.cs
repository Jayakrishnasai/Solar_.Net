using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolarSystem.Api.Data;
using SolarSystem.Shared.Models;

namespace SolarSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly SolarSystemDbContext _context;
    private readonly ILogger<ContactController> _logger;

    public ContactController(SolarSystemDbContext context, ILogger<ContactController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Submit a contact form
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ContactResponse>> SubmitContact([FromBody] ContactSubmitDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || 
            string.IsNullOrWhiteSpace(dto.Email) || 
            string.IsNullOrWhiteSpace(dto.Message))
        {
            return BadRequest(new ContactResponse 
            { 
                Success = false, 
                Message = "Name, email, and message are required." 
            });
        }

        try
        {
            var submission = new ContactSubmission
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim(),
                Subject = dto.Subject?.Trim() ?? "",
                Message = dto.Message.Trim(),
                Category = dto.Category,
                SubmittedAt = DateTime.UtcNow
            };

            _context.ContactSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Contact form submitted: {Id} from {Email}", submission.Id, submission.Email);

            return Ok(new ContactResponse
            {
                Success = true,
                Message = "Thank you for contacting us! We'll get back to you soon.",
                SubmissionId = submission.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving contact submission");
            return StatusCode(500, new ContactResponse
            {
                Success = false,
                Message = "An error occurred. Please try again later."
            });
        }
    }

    /// <summary>
    /// Get all submissions (admin endpoint)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ContactSubmission>>> GetSubmissions([FromQuery] int limit = 50)
    {
        var submissions = await _context.ContactSubmissions
            .OrderByDescending(c => c.SubmittedAt)
            .Take(limit)
            .ToListAsync();

        return Ok(submissions);
    }

    /// <summary>
    /// Get submission count
    /// </summary>
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetCount()
    {
        return await _context.ContactSubmissions.CountAsync();
    }
}

public class ContactSubmitDto
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Subject { get; set; }
    public string Message { get; set; } = "";
    public string? Category { get; set; }
}

public class ContactResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public int? SubmissionId { get; set; }
}
