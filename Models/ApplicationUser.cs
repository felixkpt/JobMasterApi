// File: Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace ResumeUploadApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public ICollection<Resume> Resumes { get; set; } = new List<Resume>();

    }
}
