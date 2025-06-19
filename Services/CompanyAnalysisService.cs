// Services/Interfaces/ICompanyAnalysisService.cs
using ResumeUploadApi.Models;

namespace ResumeUploadApi.Services.Interfaces
{
    public class CompanyAnalysisService : ICompanyAnalysisService
    {
        private readonly IGptService _gptService;

        public CompanyAnalysisService(IGptService gptService)
        {
            _gptService = gptService;
        }

        public async Task<string> GetReputationAsync(
            string userId,
            string companyName,
            string? companyWebsite
        )
        {
            return await _gptService.GetCompanyReputationAsync(companyName);
        }
    }
}
