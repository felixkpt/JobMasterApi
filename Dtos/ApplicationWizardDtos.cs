using System.ComponentModel.DataAnnotations;

namespace JobMasterApi.Dtos
{
    public class CoverLetterRequestDto
    {
        [Required(ErrorMessage = "Resume ID is required.")]
        public Guid? ResumeId { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Job title must be at least 5 characters.")]
        [MaxLength(3000, ErrorMessage = "Job title must be 100 characters or less.")]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        [MinLength(5, ErrorMessage = "Job description must be at least 5 characters.")]
        [MaxLength(3000, ErrorMessage = "Job description must be 3000 characters or less.")]
        public string JobDescription { get; set; } = string.Empty;
    }

    public class FitAnalysisRequestDto
    {
        [Required(ErrorMessage = "Resume ID is required.")]
        public Guid? ResumeId { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Job description must be at least 5 characters.")]
        [MaxLength(3000, ErrorMessage = "Job description must be 3000 characters or less.")]
        public string JobDescription { get; set; } = string.Empty;
    }

    public class JobDescriptionDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Job description must be at least 5 characters.")]
        [MaxLength(3000, ErrorMessage = "Job description must be 3000 characters or less.")]
        public string Description { get; set; } = string.Empty;
    }

    class FitResultDto
    {
        public int Score { get; set; }
        public string Insights { get; set; } = string.Empty;
    }

    public class GenerateAnswerForQuestionRequestDto
    {
        public Guid? ResumeId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
    }

    public class CoverLetterDownloadDto
    {
        public string CoverLetter { get; set; } = string.Empty;
    }
}
