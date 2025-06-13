namespace ResumeUploadApi.Dtos
{
    public class FitAnalysisRequestDto
    {
        public Guid ResumeId { get; set; }
        public string JobDescription { get; set; } = string.Empty;
    }
}
