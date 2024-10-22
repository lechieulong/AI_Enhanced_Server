using Entity;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Entity.Data;
using System;
using System.Collections.Generic;
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

        public async Task<CourseTimeline> GetByIdAsync(Guid id)
        {
            return await _context.CourseTimelines.FindAsync(id);
        }

        public async Task<IEnumerable<CourseTimeline>> GetAllAsync()
        {
            return await _context.CourseTimelines.ToListAsync();
        }

        public async Task CreateAsync(CourseTimeline courseTimeline)
        {
            if (courseTimeline == null)
            {
                throw new ArgumentNullException(nameof(courseTimeline), "CourseTimeline cannot be null.");
            }

            await _context.CourseTimelines.AddAsync(courseTimeline);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseTimeline courseTimeline)
        {
            if (courseTimeline == null)
            {
                throw new ArgumentNullException(nameof(courseTimeline), "CourseTimeline cannot be null.");
            }

            _context.CourseTimelines.Update(courseTimeline);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var timeline = await _context.CourseTimelines.FindAsync(id);
            if (timeline != null)
            {
                _context.CourseTimelines.Remove(timeline);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("CourseTimeline not found.");
            }
        }

        public async Task<bool> CheckExistCourseIdAsync(Guid courseId)
        {
            return await _context.Courses.AnyAsync(c => c.Id == courseId);
        }

        public async Task<IEnumerable<CourseTimeline>> GetByCourseIdAsync(Guid courseId)
        {
            return await _context.CourseTimelines
                .Where(ct => ct.CourseId == courseId)
                .ToListAsync();
        }
    }
}
