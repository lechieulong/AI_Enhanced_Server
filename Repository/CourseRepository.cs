using Entity;
using IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

        public async Task<Course> GetByIdAsync(int id)
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

        public async Task DeleteAsync(int id)
        {
            var course = await _courses.FindAsync(id);
            if (course != null)
            {
                _courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }

    }
}
