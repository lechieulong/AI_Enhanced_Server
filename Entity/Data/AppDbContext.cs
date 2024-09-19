using Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Model.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseTimeline> CourseTimelines { get; set; }
        public DbSet<CourseTimelineDetail> CourseTimelineDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thiết lập mối quan hệ 1-nhiều giữa Course và CourseTimeline
            modelBuilder.Entity<Course>()
                .HasMany(c => c.CourseTimelines)
                .WithOne(t => t.Course)
                .HasForeignKey(t => t.CourseId);

            // Thiết lập mối quan hệ 1-nhiều giữa CourseTimeline và CourseTimelineDetail
            modelBuilder.Entity<CourseTimeline>()
                .HasMany(t => t.CourseTimelineDetails)
                .WithOne(td => td.CourseTimeline)
                .HasForeignKey(td => td.TimelineId);
        }
    }
}
