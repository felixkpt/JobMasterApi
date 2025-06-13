using ResumeUploadApi.Dtos;
using ResumeUploadApi.Models;

namespace ResumeUploadApi.Mappers
{
    public static class ResumeMapper
    {
        public static ResumeDto ToDto(this Resume resume)
        {
            return new ResumeDto
            {
                Id = resume.Id,
                FileName = resume.FileName,
                UploadedAt = resume.UploadedAt
            };
        }

        public static IEnumerable<ResumeDto> ToDtoList(this IEnumerable<Resume> resumes)
        {
            return resumes.Select(r => r.ToDto());
        }
    }
}
