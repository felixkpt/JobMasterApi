using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResumeUploadApi.Models;
using ResumeUploadApi.Services.Interfaces;
using ResumeUploadApi.Mappers;

namespace ResumeUploadApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/resumes")]
public class ResumeController : BaseController
{
    private readonly IResumeService _resumeService;

    public ResumeController(IResumeService resumeService, UserManager<ApplicationUser> userManager)
        : base(userManager)
    {
        _resumeService = resumeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetResumes()
    {
        var userId = await GetUserIdAsync();
        var resumes = await _resumeService.GetResumesAsync(userId);

        var dto = resumes.ToDtoList();
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> UploadResume(IFormFile file)
    {
        var userId = await GetUserIdAsync();
        var resume = await _resumeService.UploadResumeAsync(userId, file);
        var dto = resume.ToDto(); // prevent serialization issues

        return CreatedAtAction(nameof(GetResumes), null, dto);
    }

    [HttpDelete("{resumeId}")]
    public async Task<IActionResult> DeleteResume(Guid resumeId)
    {
        var userId = await GetUserIdAsync();
        var success = await _resumeService.DeleteResumeAsync(userId, resumeId);
        return success ? NoContent() : NotFound("Resume not found.");
    }
}
