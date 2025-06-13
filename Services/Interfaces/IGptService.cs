namespace ResumeUploadApi.Services.Interfaces
{
	public interface IGptService
	{
		Task<int> CalculateMatchScoreAsync(string resumeContent, string jobDescription);
		Task<string> GetImprovementSuggestionsAsync(string resumeContent, string jobDescription);
		Task<string> GenerateCoverLetterAsync(string userId, Guid resumeId, string jobDescription);
		Task<(int Score, string Insights)> AnalyzeJobFitAsync(string userId, Guid resumeId, string jobDescription);
    }
}
