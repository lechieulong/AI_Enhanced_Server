using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity;
using Entity.Data;
using IRepository;
using Microsoft.EntityFrameworkCore;

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
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course> GetByIdAsync(Guid id)
        {
            return await _context.Courses.FindAsync(id);
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
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
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

        public async Task UpdateCourseEnabledStatusAsync(Guid courseId, bool isEnabled)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course != null)
            {
                course.IsEnabled = isEnabled;
                await _context.SaveChangesAsync();
            }
        }
    }
}
