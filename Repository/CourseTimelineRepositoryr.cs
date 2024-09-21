using Entity;
using IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Entity;
using Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CourseTimelineRepository : ICourseTimelineRepository
    {
        private readonly AppDbContext _context;

        public CourseTimelineRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CourseTimeline> GetByIdAsync(int id)
        {
            return await _context.CourseTimelines.FindAsync(id);
        }

        public async Task<IEnumerable<CourseTimeline>> GetAllAsync()
        {
            return await _context.CourseTimelines.ToListAsync();
        }

        public async Task AddAsync(CourseTimeline courseTimeline)
        {
            await _context.CourseTimelines.AddAsync(courseTimeline);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseTimeline courseTimeline)
        {
            _context.CourseTimelines.Update(courseTimeline);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var timeline = await _context.CourseTimelines.FindAsync(id);
            if (timeline != null)
            {
                _context.CourseTimelines.Remove(timeline);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CheckExistCourseIdAsync(int courseId)
        {
            return await _context.Courses.AnyAsync(c => c.Id == courseId);
        }

        public async Task<IEnumerable<CourseTimeline>> GetByCourseIdAsync(int courseId)
        {
            return await _context.CourseTimelines.Where(ct => ct.CourseId == courseId).ToListAsync();
        }
    }
}
