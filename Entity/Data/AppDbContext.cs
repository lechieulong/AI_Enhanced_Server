using Entity.Live;
using Entity.Test;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        public DbSet<UserEducation> UserEducations { get; set; }
        public DbSet<Specialization> Specializations { get; set; }
        public DbSet<Class> Classes { get; set; }
        // Test Exam 
        public DbSet<TestExam> TestExams { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<SectionQuestion> SectionQuestion { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        //End Test Exam 
        public DbSet<ClassRelationShip> ClassRelationShip { get; set; }

        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseTimeline> CourseTimelines { get; set; }
        public DbSet<CourseTimelineDetail> CourseTimelineDetails { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<TeacherAvailableSchedule> TeacherAvailableSchedules { get; set; }
        public DbSet<BookedTeacherSession> BookedTeacherSessions { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Transaction>  Transactions{ get; set; }
        public DbSet<AccountBalance> AccountBalances { get; set; }
        public DbSet<LiveStream> LiveStreams { get; set; }
        public DbSet<StreamSession> StreamSessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User_Ticket> User_Tickets { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Thêm dữ liệu mặc định vào bảng Specializations
            //modelBuilder.Entity<Specialization>().HasData(
            //    new Specialization { Id = Guid.NewGuid(), Name = "Speaking" },
            //    new Specialization { Id = Guid.NewGuid(), Name = "Writing" },
            //    new Specialization { Id = Guid.NewGuid(), Name = "Reading" },
            //    new Specialization { Id = Guid.NewGuid(), Name = "Listening" }
            //);

            modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.TeacherAvailableSchedules)
            .WithOne(s => s.Teacher) // Mỗi lịch chỉ có một giáo viên
            .HasForeignKey(s => s.TeacherId)
            .OnDelete(DeleteBehavior.Restrict); // Không xóa lịch khi xóa giáo viên

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.BookedTeacherSessions)
                .WithOne(b => b.Learner) // Mỗi phiên học chỉ có một học viên
                .HasForeignKey(b => b.LearnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeacherAvailableSchedule>()
                .Property(t => t.Price)
                .HasColumnType("decimal(18,4)");

            // Cấu hình quan hệ Course - Class
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Course)
                .WithMany(c => c.Classes)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.NoAction); // Tắt cascade khi xóa

            // Cấu hình quan hệ Class - Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Class)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.NoAction); // Tắt cascade khi xóa

            // Cấu hình quan hệ Course - Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.NoAction); // Tắt cascade khi xóa

            // Cấu hình quan hệ User - Enrollment
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction); // Tắt cascade khi xóa
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Events)
                .WithMany(u => u.Users)
                .UsingEntity(j => j.ToTable("UserEvents"));

            modelBuilder.Entity<UserEducation>()
                .HasMany(ue => ue.Specializations)
                .WithMany(s => s.UserEducations)
                .UsingEntity(j => j.ToTable("UserEducationSpecializations"));
        }

    }
}
