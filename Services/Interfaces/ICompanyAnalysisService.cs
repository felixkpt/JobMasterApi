// Services/Interfaces/ICompanyAnalysisService.cs
using ResumeUploadApi.Models;

namespace ResumeUploadApi.Services.Interfaces
{
    public interface ICompanyAnalysisService
    {
        Task<string> GetReputationAsync(string userId, string companyName, string? companyWebsite);
    }
}
