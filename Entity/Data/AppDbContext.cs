﻿using Entity.Test;
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
        public DbSet<Section> Section { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Answer> Answer { get; set; }
        public DbSet<ClassRelationShip> ClassRelationShip { get; set; }

        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseTimeline> CourseTimelines { get; set; }
        public DbSet<CourseTimelineDetail> CourseTimelineDetails { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<TeacherAvailableSchedule> TeacherAvailableSchedules { get; set; }
        public DbSet<BookedTeacherSession> BookedTeacherSessions { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }

    }
}
