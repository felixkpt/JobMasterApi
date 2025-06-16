namespace ResumeUploadApi.Models
{
    public class CoverLetter
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = null!;
        public Guid ResumeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public int? MatchScore { get; set; }
        public string? Insights { get; set; }
 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
