using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeUploadApi.Models
{
    public class Resume
    {
        public Guid Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public ApplicationUser User { get; set; } = default!;
    }
}
