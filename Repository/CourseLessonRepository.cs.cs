using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entity.CourseFolder;
using Entity.Data;
using IRepository;

namespace Repository
{
    public class CourseLessonRepository : ICourseLessonRepository
    {
        private readonly AppDbContext _context;

        public CourseLessonRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourseLesson>> GetAllAsync()
        {
            return await _context.Set<CourseLesson>().ToListAsync();
        }

        public async Task<CourseLesson> GetByIdAsync(Guid id)
        {
            return await _context.Set<CourseLesson>().FindAsync(id);
        }

        public async Task<CourseLesson> AddAsync(CourseLesson courseLesson)
        {
            await _context.Set<CourseLesson>().AddAsync(courseLesson);
            await _context.SaveChangesAsync();
            return courseLesson;
        }

        public async Task<CourseLesson> UpdateAsync(CourseLesson courseLesson)
        {
            _context.Set<CourseLesson>().Update(courseLesson);
            await _context.SaveChangesAsync();
            return courseLesson;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var courseLesson = await _context.Set<CourseLesson>().FindAsync(id);
            if (courseLesson == null)
                return false;

            _context.Set<CourseLesson>().Remove(courseLesson);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<CourseLesson>> GetByCoursePartIdAsync(Guid coursePartId)
        {
            return await _context.CourseLessons
                .Where(cl => cl.CoursePartId == coursePartId)
                .ToListAsync();

        }

        public async Task<int> GetMaxOrderByCoursePartIdAsync(Guid coursePartId)
        {
            return await _context.CourseLessons
                .Where(cp => cp.CoursePartId == coursePartId)
                .Select(cp => (int?)cp.Order) // Sử dụng nullable để tránh lỗi khi không có giá trị
                .MaxAsync() ?? 0; // Trả về 0 nếu không có giá trị nào
        }
    }
}