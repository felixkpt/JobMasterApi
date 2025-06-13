// File: Data/AppDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResumeUploadApi.Models;

namespace ResumeUploadApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Resume> Resumes => Set<Resume>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Resume>()
                .HasOne(r => r.User)              // use the navigation property
                .WithMany(u => u.Resumes)         // assuming you add this collection to ApplicationUser
                .HasForeignKey(r => r.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
