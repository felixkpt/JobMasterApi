using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResumeUploadApi.Dtos;
using ResumeUploadApi.Models;
using ResumeUploadApi.Services.Interfaces;
using Xceed.Words.NET;

namespace ResumeUploadApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/application-wizard")]
public class ApplicationWizardController : BaseController
{
    private readonly IResumeService _resumeService;
    private readonly IGptService _gptService;

    public ApplicationWizardController(IResumeService resumeService, IGptService gptService, UserManager<ApplicationUser> userManager
): base(userManager)
    {
        _resumeService = resumeService;
        _gptService = gptService;
    }

    [HttpPost("analyze-fit")]
    public async Task<IActionResult> AnalyzeFit([FromBody] FitAnalysisRequestDto request)
    {
        try
        {
		    var userId = await GetUserIdAsync();
            var result = await _gptService.AnalyzeJobFitAsync(
                userId,
                request.ResumeId,
                request.JobDescription
            );
            return Ok(new { fitScore = result.Score, insights = result.Insights });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("generate-cover-letter")]
    public async Task<IActionResult> GenerateCoverLetter([FromBody] CoverLetterRequestDto request)
    {
        try
        {
            var userId = await GetUserIdAsync();
            var letter = await _gptService.GenerateCoverLetterAsync(
                userId,
                request.ResumeId,
                request.JobDescription
            );
            return Ok(new { coverLetter = letter });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("download-cover-letter-docx")]
    public IActionResult DownloadCoverLetterAsDocx(string coverLetter)
    {
        using var stream = new MemoryStream();
        using (var doc = DocX.Create(stream))
        {
            doc.InsertParagraph("Generated Cover Letter").Bold().FontSize(14);
            doc.InsertParagraph(coverLetter);
            doc.Save();
        }

        stream.Position = 0;
        return File(
            stream,
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "cover-letter.docx"
        );
    }
}
