// File: Services/Interfaces/IResumeService.cs
using ResumeUploadApi.Models;

namespace ResumeUploadApi.Services.Interfaces
{
    public interface IResumeService
    {
        Task<IEnumerable<Resume>> GetResumesAsync(string userId);
        Task<Resume> UploadResumeAsync(string userId, IFormFile file);
        Task<bool> DeleteResumeAsync(string userId, Guid resumeId);
    }
}
