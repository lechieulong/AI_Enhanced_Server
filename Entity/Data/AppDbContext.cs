using Entity.Test;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entity.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<TestExam> TestExam { get; set; }
        public DbSet<SkillTestExam> SkillTestExam { get; set; }
        public DbSet<PartSkill> PartSkill { get; set; }
        public DbSet<QuestionTypePart> QuestionTypePart { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<AnswerOptions> AnswerOptions { get; set; }
        public DbSet<AnswerMatching> AnswerMatching { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseTimeline> CourseTimelines { get; set; }
        public DbSet<CourseTimelineDetail> CourseTimelineDetails { get; set; }

        public DbSet<EmailLog> EmailLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
