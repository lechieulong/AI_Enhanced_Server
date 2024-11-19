using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using Entity.Data;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Entity.CourseFolder;
namespace Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(course => course.User)
                .Where(course => course.IsEnabled)
                .ToListAsync();
        }

        public async Task<Course> GetByIdAsync(Guid id)
        {
            return await _context.Courses
                .Include(c => c.User) // Include the related User entity
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            var existingCourse = await _context.Courses.FindAsync(course.Id);
            if (existingCourse != null)
            {
                existingCourse.CourseName = course.CourseName;
                existingCourse.Content = course.Content;
                existingCourse.Hours = course.Hours;
                existingCourse.Days = course.Days;

                // Update categories
                existingCourse.Categories = course.Categories;

                existingCourse.Price = course.Price;
                existingCourse.IsEnabled = course.IsEnabled;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var course = await _context.Courses
                .Include(c => c.Classes) 
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {id} not found.");
            }


            _context.Classes.RemoveRange(course.Classes);

            _context.Courses.Remove(course);

            await _context.SaveChangesAsync();
        }


        public async Task<List<Course>> GetAllEnabledCoursesAsync()
        {
            return await _context.Courses.Where(c => c.IsEnabled).ToListAsync();
        }

        public async Task<List<Course>> GetAllDisabledCoursesAsync()
        {
            return await _context.Courses.Where(c => !c.IsEnabled).ToListAsync();
        }

        public async Task<List<Course>> GetAllCourseByUserIdAsync(string userId)
        {
            return await _context.Courses.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<List<Course>> GetCreatedCourses(string userId)
        {
            return await _context.Courses
                .Include(course => course.User)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateCourseEnabledStatusAsync(Guid courseId, bool isEnabled)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
            {
                course.IsEnabled = isEnabled;
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Guid?> GetCourseIdByLessonContentIdAsync(Guid courseLessonContentId)
        {
            var courseId = await (from clc in _context.CourseLessonContents
                                  join cl in _context.CourseLessons on clc.CourseLessonId equals cl.Id
                                  join cp in _context.CourseParts on cl.CoursePartId equals cp.Id
                                  join cs in _context.CourseSkills on cp.CourseSkillId equals cs.Id
                                  join c in _context.Courses on cs.CourseId equals c.Id
                                  where clc.Id == courseLessonContentId
                                  select c.Id)
                                 .FirstOrDefaultAsync();

            return courseId == Guid.Empty ? (Guid?)null : courseId;
        }

        public async Task<bool> CheckLecturerOfCourse(string userId, Guid courseId)
        {
            try
            {
                var course = await _context.Courses
                    .Where(c => c.Id == courseId)
                    .FirstOrDefaultAsync();

                if (course == null)
                {
                    return false;
                }

                return course.UserId == userId;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
