using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JobMasterApi.Dtos;
using JobMasterApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using ResumeUploadApi.Data;
using ResumeUploadApi.Services.Interfaces;

namespace ResumeUploadApi.Services
{
    public class GptService : IGptService
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _httpClient;
        private readonly string _openAiKey;
        private readonly string _model;
        private readonly bool _IsProd;
        private readonly int _TruncateCharsTo = 200;

        public GptService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _httpClient = new HttpClient();
            _openAiKey = config["OpenAI:ApiKey"] ?? throw new Exception("OpenAI API key missing");
            _model = config["OpenAI:Model"] ?? throw new Exception("OpenAI API Model missing");

            _IsProd = bool.TryParse(config["IsProd"], out var isProd)
                ? isProd
                : throw new Exception("IsProd config value missing or invalid");
        }

        public async Task<string> GenerateCoverLetterAsync(
            string userId,
            Guid resumeId,
            string jobDescription
        )
        {
            var resume = await _db
                .Resumes.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.Id == resumeId);

            if (resume == null)
                throw new AppException("Resume not found for this user.");

            var userName = resume.User.FullName;
            var today = DateTime.UtcNow.ToString("MMMM dd, yyyy");

            var prompt =
                @$"
You are a professional career assistant. Based on the following resume and job description, write a compelling and concise cover letter. The letter source should contain {userName}, be dated {today} and signed by the applicant, {userName}. Address it to the hiring manager.

{(!_IsProd ? "Keep response at max " + _TruncateCharsTo + " characters." : "")}

Resume:
{Truncate(resume.Content)}

Job Description:
{Truncate(jobDescription)}

Cover Letter:
";

            return await CallOpenAiAsync(prompt);
        }

        public async Task<(int Score, string Insights)> AnalyzeJobFitAsync(
            string userId,
            Guid resumeId,
            string jobDescription
        )
        {
            var resume = await _db
                .Resumes.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.Id == resumeId);

            if (resume == null)
                throw new AppException("Resume not found for this user.");

            var prompt =
                @$"
You are a resume analysis assistant. Based on the resume and job description below, provide a match score (from 0 to 100) and a brief explanation why:

{(!_IsProd ? "Keep response at max " + _TruncateCharsTo + " characters." : "")}

Resume:
{Truncate(resume.Content)}

Job Description:
{Truncate(jobDescription)}

Respond in JSON format like:
{{ ""score"": 85, ""insights"": ""Your experience closely matches the job's requirements..."" }}
";

            var response = await CallOpenAiAsync(prompt);
            try
            {
                var result = JsonSerializer.Deserialize<FitResultDto>(
                    response,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                return (result?.Score ?? 0, result?.Insights ?? "No insights provided.");
            }
            catch
            {
                return (0, "Failed to parse AI response.");
            }
        }

        public async Task<string> GenerateAnswerForQuestionAsync(
            string userId,
            GenerateAnswerForQuestionRequestDto context
        )
        {
            var resume = await _db
                .Resumes.Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.Id == context.ResumeId);

            if (resume == null)
                throw new AppException("Resume not found for this user.");

            var prompt = $"""
                You are an experienced software engineer applying for the position of "{context.JobTitle}".
                Based on the job description and your resume, write a professional and confident answer to the following interview question.
                Respond in the first person, as if you're speaking directly to the interviewer.

                {(!_IsProd ? "Keep the answer under " + _TruncateCharsTo + " characters. " : "")}

                Job Description:
                {Truncate(context.JobDescription)}

                Resume:
                {Truncate(resume.Content)}

                Interview Question:
                {context.Question}

                Begin your answer:
                """;

            return await CallOpenAiAsync(prompt);
        }

        private async Task<string> CallOpenAiAsync(string prompt)
        {
            var payload = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = prompt },
                },
                temperature = 0.7,
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.openai.com/v1/chat/completions"
            );
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _openAiKey);
            request.Content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var resultJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(resultJson);
            return doc
                .RootElement.GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
        }

        public async Task<int> CalculateMatchScoreAsync(string resumeText, string jobDescription)
        {
            // Optionally deprecated if `AnalyzeJobFitAsync` does both
            return (
                await AnalyzeJobFitAsync(Guid.Empty.ToString(), Guid.Empty, jobDescription)
            ).Score;
        }

        public async Task<string> GetImprovementSuggestionsAsync(
            string resumeText,
            string jobDescription
        )
        {
            // Optional future method if needed
            var prompt =
                @$"
As a resume advisor, provide improvement suggestions for the resume based on the following job description:

{(!_IsProd ? "Keep response at max " + _TruncateCharsTo + " characters." : "")}

Resume:
{Truncate(resumeText)}

Job Description:
{Truncate(jobDescription)}

Suggestions:
";

            return await CallOpenAiAsync(prompt);
        }

        private string Truncate(string input)
        {
            return string.IsNullOrEmpty(input)
                ? ""
                : (
                    !_IsProd && input.Length > _TruncateCharsTo
                        ? input.Substring(0, _TruncateCharsTo) + "..."
                        : input
                );
        }
    }
}
