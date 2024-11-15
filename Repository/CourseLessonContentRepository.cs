using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity.CourseFolder;
using Entity.Data;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CourseLessonContentRepository : ICourseLessonContentRepository
    {
        private readonly AppDbContext _context;

        public CourseLessonContentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourseLessonContent>> GetAllAsync()
        {
            return await _context.Set<CourseLessonContent>().ToListAsync();
        }

        public async Task<CourseLessonContent> GetByIdAsync(Guid id)
        {
            return await _context.Set<CourseLessonContent>().FindAsync(id);
        }

        public async Task<IEnumerable<CourseLessonContent>> GetByCourseLessonIdAsync(Guid courseLessonId)
        {
            return await _context.Set<CourseLessonContent>()
                .Where(c => c.CourseLessonId == courseLessonId)
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        public async Task<CourseLessonContent> AddAsync(CourseLessonContent courseLessonContent)
        {
            await _context.Set<CourseLessonContent>().AddAsync(courseLessonContent);
            await _context.SaveChangesAsync();
            return courseLessonContent;
        }

        public async Task UpdateAsync(CourseLessonContent courseLessonContent)
        {
            _context.Entry(courseLessonContent).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var content = await _context.Set<CourseLessonContent>().FindAsync(id);
            if (content != null)
            {
                _context.Set<CourseLessonContent>().Remove(content);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<CourseLessonContent>> GetByCourseLessonId(Guid courseLessonId)
        {
            return await _context.CourseLessonContents
                .Where(c => c.CourseLessonId == courseLessonId)
                .OrderBy(c => c.Order) // Optional: Order by the `Order` field
                .ToListAsync();
        }
    }
}
