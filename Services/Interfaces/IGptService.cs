using JobMasterApi.Dtos;

namespace ResumeUploadApi.Services.Interfaces
{
    public interface IGptService
    {
        Task<int> CalculateMatchScoreAsync(string resumeContent, string jobDescription);
        Task<string> GetImprovementSuggestionsAsync(string resumeContent, string jobDescription);
        Task<string> GenerateCoverLetterAsync(string userId, Guid resumeId, string jobDescription);
        Task<string> GenerateAnswerForQuestionAsync(
            string userId,
            GenerateAnswerForQuestionRequestDto context
        );
        Task<(int Score, string Insights)> AnalyzeJobFitAsync(
            string userId,
            Guid resumeId,
            string jobDescription
        );
        Task<string> GetCompanyReputationAsync(string companyName, string? companyWebsite = null);
    }
}
