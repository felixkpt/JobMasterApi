using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeUploadApi.Data;
using ResumeUploadApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ResumeUploadApi.Controllers;

[ApiController]
[Route("users/{userId}/resumes")]
public class ResumeController : ControllerBase
{
    private readonly AppDbContext _db;

    public ResumeController(AppDbContext db)
    {
        _db = db;
    }

    // GET: /users/{userId}/resumes
    [HttpGet]
    public async Task<IActionResult> GetResumes(int userId)
    {
        var resumes = await _db.Resumes
            .Where(r => r.UserId == userId)
            .ToListAsync();

        return Ok(resumes);
    }

    // POST: /users/{userId}/resumes
    [HttpPost]
    public async Task<IActionResult> UploadResume(int userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var uploadsFolder = Path.Combine("Uploads", $"user-{userId}");
        Directory.CreateDirectory(uploadsFolder);

        var filePath = Path.Combine(uploadsFolder, file.FileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var resume = new Resume
        {
            UserId = userId,
            FileName = file.FileName,
            UploadedAt = DateTime.UtcNow
        };

        _db.Resumes.Add(resume);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetResumes), new { userId }, resume);
    }

    [Authorize]
    [HttpGet("secure-data")]
    public IActionResult GetSecureData() => Ok("You are authorized!");

}
