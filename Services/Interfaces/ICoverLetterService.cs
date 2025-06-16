// Services/Interfaces/ICoverLetterService.cs
using ResumeUploadApi.Models;

namespace ResumeUploadApi.Services.Interfaces
{
    public interface ICoverLetterService
    {
        Task<CoverLetter> GenerateAndSaveAsync(
            string userId,
            string jobTitle,
            string jobDescription,
            Guid resumeId
        );
        Task<string> GenerateAnswerForQuestion(
            string userId,
            Guid resumeId,
            string jobTitle,
            string jobDescription,
            string question
        );
        Task<MemoryStream> ExportAsync(Guid coverLetterId, string userId, string format); // pdf | docx | txt
    }
}
