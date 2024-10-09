using Entity;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository
{
    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Course> _courses;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
            _courses = _context.Courses;
        }

        public async Task<Course> GetByIdAsync(Guid id) // Thay đổi từ int sang Guid
        {
            return await _courses.FindAsync(id);
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _courses.ToListAsync();
        }

        public async Task AddAsync(Course course)
        {
            await _courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id) // Thay đổi từ int sang Guid
        {
            var course = await _courses.FindAsync(id);
            if (course != null)
            {
                _courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Course>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Courses
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }
    }
}
