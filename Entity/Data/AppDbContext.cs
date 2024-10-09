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
        public DbSet<UserClass> UserClasses { get; set; }
        public DbSet<UserCourse> UserCourses { get; set; } // Thêm DbSet cho UserCourse

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserCourse>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCourses)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserClass>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserClasses)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
