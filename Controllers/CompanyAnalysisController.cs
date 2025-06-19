using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResumeUploadApi.Models;
using ResumeUploadApi.Services.Interfaces;

namespace ResumeUploadApi.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/company-analysis")]
public class CompanyAnalysisController : BaseController
{
    private readonly ICompanyAnalysisService _companyAnalysisService;

    public CompanyAnalysisController(
        UserManager<ApplicationUser> userManager,
        ICompanyAnalysisService companyAnalysisService
    )
        : base(userManager)
    {
        _companyAnalysisService = companyAnalysisService;
    }

    [HttpGet("get-reputation")]
    public async Task<IActionResult> GetReputation([FromQuery] string companyName)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            return BadRequest("Company name is required.");

        var userId = await GetUserIdAsync();

        var reputation = await _companyAnalysisService.GetReputationAsync(userId, companyName, "");

        return Ok(new { result = reputation });
    }
}
