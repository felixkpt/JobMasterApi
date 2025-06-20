// Services/CoverLetterService.cs
using JobMasterApi.Dtos;
using JobMasterApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using ResumeUploadApi.Data;
using ResumeUploadApi.Models;
using ResumeUploadApi.Services.Interfaces;

namespace ResumeUploadApi.Services
{
    public class CoverLetterService : ICoverLetterService
    {
        private readonly AppDbContext _dbContext;
        private readonly IGptService _gptService;

        public CoverLetterService(AppDbContext dbContext, IGptService gptService)
        {
            _dbContext = dbContext;
            _gptService = gptService;
        }

        public async Task<CoverLetter> GenerateAndSaveAsync(
            string userId,
            string jobTitle,
            string jobDescription,
            Guid resumeId
        )
        {
            var fitResult = await _gptService.AnalyzeJobFitAsync(userId, resumeId, jobDescription);
            var content = await _gptService.GenerateCoverLetterAsync(
                userId,
                resumeId,
                jobDescription
            );

            var letter = new CoverLetter
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ResumeId = resumeId,
                Title = $"Cover Letter for {jobTitle}",
                JobDescription = jobDescription,
                Content = content,
                MatchScore = fitResult.Score,
                Insights = fitResult.Insights,
                CreatedAt = DateTime.UtcNow,
            };

            _dbContext.CoverLetters.Add(letter);
            await _dbContext.SaveChangesAsync();

            return letter;
        }

        public async Task<MemoryStream> ExportAsync(
            Guid coverLetterId,
            string userId,
            string format
        )
        {
            var letter = await _dbContext.CoverLetters.FirstOrDefaultAsync(c =>
                c.Id == coverLetterId && c.UserId == userId
            );

            if (letter == null)
                throw new AppException("Cover letter not found.");

            format = format?.ToLowerInvariant() ?? string.Empty;

            return format switch
            {
                "pdf" => ExportAsPdf(letter),
                "docx" => ExportAsDocx(letter),
                "txt" => ExportAsTxt(letter),
                _ => throw new AppException("Unsupported format. Use pdf, docx, or txt."),
            };
        }

        private static MemoryStream ExportAsPdf(CoverLetter letter)
        {
            var stream = new MemoryStream();

            Document
                .Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(50);
                        page.Content()
                            .Column(col =>
                            {
                                col.Item().PaddingVertical(10);
                                col.Item().Text(letter.Content).FontSize(12);
                            });
                    });
                })
                .GeneratePdf(stream);

            stream.Position = 0;
            return stream;
        }

        private static MemoryStream ExportAsDocx(CoverLetter letter)
        {
            var stream = new MemoryStream();

            using (var doc = Xceed.Words.NET.DocX.Create(stream))
            {
                doc.InsertParagraph(letter.Content).FontSize(12);
                doc.Save();
            }

            stream.Position = 0;
            return stream;
        }

        public async Task<string> GenerateAnswerForQuestion(
            string userId,
            Guid resumeId,
            string jobTitle,
            string jobDescription,
            string question
        )
        {
            var resume = await _dbContext
                .Resumes.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == resumeId && r.UserId == userId);

            if (resume == null)
                throw new AppException("Resume not found for this user.");

            var context = new GenerateAnswerForQuestionRequestDto
            {
                ResumeId = resume.Id,
                JobTitle = jobTitle,
                JobDescription = jobDescription,
                Question = question,
            };

            return await _gptService.GenerateAnswerForQuestionAsync(userId, context);
        }

        private static MemoryStream ExportAsTxt(CoverLetter letter)
        {
            var stream = new MemoryStream();

            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.WriteLine(letter.Content);
                writer.Flush();
            }

            stream.Position = 0;
            return stream;
        }
    }
}
