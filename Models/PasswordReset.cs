namespace ResumeUploadApi.Models
{
    public class PasswordReset
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public string Token { get; set; } = null!; // JWT or random string

        public string? Code { get; set; } // Optional: 6-digit SMS/email code

        public DateTime ExpiresAt { get; set; }

        public bool Used { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
