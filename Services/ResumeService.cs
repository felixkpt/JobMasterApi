// File: Services/ResumeService.cs
using JobMasterApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using ResumeUploadApi.Data;
using ResumeUploadApi.Models;
using ResumeUploadApi.Services.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace ResumeUploadApi.Services
{
    public class ResumeService : IResumeService
    {
        private readonly AppDbContext _db;

        public ResumeService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Resume>> GetResumesAsync(string userId)
        {
            return await _db.Resumes
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<Resume> UploadResumeAsync(string userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new AppException("Invalid file.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Resumes", userId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.FileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Extract text from PDF
            string extractedText = "";
            using (var pdf = PdfDocument.Open(filePath))
            {
                foreach (Page page in pdf.GetPages())
                {
                    extractedText += page.Text + "\n";
                }
            }

            var resume = new Resume
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FileName = file.FileName,
                FilePath = filePath,
                UploadedAt = DateTime.UtcNow,
                Content = extractedText
            };

            _db.Resumes.Add(resume);
            await _db.SaveChangesAsync();

            return resume;
        }

        public async Task<bool> DeleteResumeAsync(string userId, Guid resumeId)
        {
            var resume = await _db.Resumes
                .FirstOrDefaultAsync(r => r.Id == resumeId && r.UserId == userId);

            if (resume == null)
                return false;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Resumes", userId.ToString());
            var filePath = Path.Combine(uploadsFolder, resume.FileName);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _db.Resumes.Remove(resume);
            await _db.SaveChangesAsync();

            return true;
        }
    }
}
