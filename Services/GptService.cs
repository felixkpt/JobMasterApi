using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ResumeUploadApi.Data;
using ResumeUploadApi.Services.Interfaces;

namespace ResumeUploadApi.Services
{
    public class GptService : IGptService
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _httpClient;
        private readonly string _openAiKey;

        public GptService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _httpClient = new HttpClient();
            _openAiKey = config["OpenAI:ApiKey"] ?? throw new Exception("OpenAI API key missing");
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
                throw new ArgumentException("Resume not found for this user.");

            var userName = resume.User.FullName;
            var today = DateTime.UtcNow.ToString("MMMM dd, yyyy");

            var prompt =
                @$"
You are a professional career assistant. Based on the following resume and job description, write a compelling and concise cover letter. The letter source should contain {userName}, be dated {today} and signed by the applicant, {userName}. Address it to the hiring manager.

Resume:
{resume.Content}

Job Description:
{jobDescription}

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
                throw new ArgumentException("Resume not found for this user.");

            var prompt =
                @$"
You are a resume analysis assistant. Based on the resume and job description below, provide a match score (from 0 to 100) and a brief explanation why:

Resume:
{resume.Content}

Job Description:
{jobDescription}

Respond in JSON format like:
{{ ""score"": 85, ""insights"": ""Your experience closely matches the job's requirements..."" }}
";

            var response = await CallOpenAiAsync(prompt);
            try
            {
                var result = JsonSerializer.Deserialize<FitResult>(
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

        private async Task<string> CallOpenAiAsync(string prompt)
        {
            var payload = new
            {
                model = "gpt-4",
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

        private class FitResult
        {
            public int Score { get; set; }
            public string Insights { get; set; }
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

Resume:
{resumeText}

Job Description:
{jobDescription}

Suggestions:
";

            return await CallOpenAiAsync(prompt);
        }
    }
}
