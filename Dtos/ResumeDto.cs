namespace ResumeUploadApi.Dtos
{
    public class ResumeDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
