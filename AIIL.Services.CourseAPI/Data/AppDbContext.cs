using AIIL.Services.CourseAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace AIIL.Services.CourseAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed data for Courses
            modelBuilder.Entity<Course>().HasData(
                new Course
                {
                    CourseId = 1,
                    Title = "Introduction to IELTS",
                    Description = "A beginner's course for IELTS preparation.",
                    Price = 99.99m,
                    ImageUrl = "https://example.com/images/ielts-intro.jpg",
                    CategoryId = 1,  // Assuming category with ID 1 exists
                    MentorId = 1     // Assuming mentor with ID 1 exists
                },
                new Course
                {
                    CourseId = 2,
                    Title = "Advanced IELTS Techniques",
                    Description = "An advanced course for students looking to master the IELTS exam.",
                    Price = 149.99m,
                    ImageUrl = "https://example.com/images/ielts-advanced.jpg",
                    CategoryId = 2,  // Assuming category with ID 2 exists
                    MentorId = 2     // Assuming mentor with ID 2 exists
                },
                new Course
                {
                    CourseId = 3,
                    Title = "IELTS Writing Mastery",
                    Description = "Focused on helping students improve their writing skills for IELTS.",
                    Price = 79.99m,
                    ImageUrl = "https://example.com/images/ielts-writing.jpg",
                    CategoryId = 1,  // Assuming category with ID 1 exists
                    MentorId = 1     // Assuming mentor with ID 1 exists
                }
            );
        }
    }
}
