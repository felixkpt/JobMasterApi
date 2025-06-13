namespace ResumeUploadApi.Dtos
{
    public class CoverLetterRequestDto
    {
        public Guid ResumeId { get; set; }

        /// <summary>
        /// The job description text that the GPT service will use to generate the cover letter.
        /// </summary>
        public string JobDescription { get; set; } = string.Empty;
    }
}
