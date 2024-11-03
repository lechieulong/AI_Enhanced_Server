using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entity.CourseFolder;
using Entity.Data;

namespace Repository
{
    public class CoursePartRepository : ICoursePartRepository
    {
        private readonly AppDbContext _context;

        public CoursePartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CoursePart>> GetAllAsync()
        {
            return await _context.Set<CoursePart>().ToListAsync();
        }

        public async Task<CoursePart> GetByIdAsync(Guid id)
        {
            return await _context.Set<CoursePart>().FindAsync(id);
        }

        public async Task<CoursePart> AddAsync(CoursePart coursePart)
        {
            await _context.Set<CoursePart>().AddAsync(coursePart);
            await _context.SaveChangesAsync();
            return coursePart;
        }

        public async Task<CoursePart> UpdateAsync(CoursePart coursePart)
        {
            _context.Set<CoursePart>().Update(coursePart);
            await _context.SaveChangesAsync();
            return coursePart;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var coursePart = await _context.Set<CoursePart>().FindAsync(id);
            if (coursePart == null)
                return false;

            _context.Set<CoursePart>().Remove(coursePart);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
