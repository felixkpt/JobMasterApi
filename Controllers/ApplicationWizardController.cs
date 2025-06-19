using JobMasterApi.Dtos;
using JobMasterApi.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResumeUploadApi.Models;
using ResumeUploadApi.Services.Interfaces;

namespace ResumeUploadApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/application-wizard")]
public class ApplicationWizardController : BaseController
{
    private readonly ICoverLetterService _coverLetterService;

    public ApplicationWizardController(
        ICoverLetterService coverLetterService,
        UserManager<ApplicationUser> userManager
    )
        : base(userManager)
    {
        _coverLetterService = coverLetterService;
    }

    [HttpPost("generate-cover-letter")]
    public async Task<IActionResult> GenerateAndSaveCoverLetter(
        [FromBody] CoverLetterRequestDto request
    )
    {
        if (request.ResumeId == null)
            throw new AppException("Resume ID is required");

        var userId = await GetUserIdAsync();

        var coverLetter = await _coverLetterService.GenerateAndSaveAsync(
            userId,
            request.JobTitle,
            request.JobDescription,
            request.ResumeId.Value
        );

        return Ok(
            new
            {
                id = coverLetter.Id,
                title = coverLetter.Title,
                coverLetter = coverLetter.Content,
                matchScore = coverLetter.MatchScore,
                insights = coverLetter.Insights,
            }
        );
    }

    [HttpPost("generate-answer-for-question")]
    public async Task<IActionResult> GenerateAnswerForQuestion(
        [FromBody] GenerateAnswerForQuestionRequestDto request
    )
    {
        if (
            !request.ResumeId.HasValue
            || request.ResumeId == Guid.Empty
            || string.IsNullOrWhiteSpace(request.Question)
        )
            return BadRequest("ResumeId and Question are required.");

        var userId = await GetUserIdAsync();

        var answer = await _coverLetterService.GenerateAnswerForQuestion(
            userId,
            request.ResumeId.Value,
            request.JobTitle,
            request.JobDescription,
            request.Question
        );

        return Ok(new { answer });
    }

    [HttpGet("download-cover-letter/{id}")]
    public async Task<IActionResult> DownloadCoverLetter(Guid id, [FromQuery] string format = "pdf")
    {
        var userId = await GetUserIdAsync();
        var stream = await _coverLetterService.ExportAsync(id, userId, format);

        string contentType = format switch
        {
            "pdf" => "application/pdf",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "txt" => "text/plain",
            _ => "application/octet-stream",
        };

        return File(stream, contentType, $"cover-letter-{id}.{format}");
    }
}
