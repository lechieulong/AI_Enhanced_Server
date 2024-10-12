using Entity;
using IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model;
using Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CourseTimelineDetailRepository : ICourseTimelineDetailRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<CourseTimelineDetail> _coursesTimelineDetail;

        public CourseTimelineDetailRepository(AppDbContext context)
        {
            _context = context;
            _coursesTimelineDetail = _context.CourseTimelineDetails;
        }

        public async Task<CourseTimelineDetail> GetByIdAsync(Guid id)
        {
            return await _coursesTimelineDetail.FindAsync(id);
        }

        public async Task<IEnumerable<CourseTimelineDetail>> GetAllAsync()
        {
            return await _coursesTimelineDetail.ToListAsync();
        }

        public async Task AddAsync(CourseTimelineDetail courseTimelineDetail)
        {
            await _coursesTimelineDetail.AddAsync(courseTimelineDetail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseTimelineDetail courseTimelineDetail)
        {
            _coursesTimelineDetail.Update(courseTimelineDetail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var courseTimelineDetail = await _coursesTimelineDetail.FindAsync(id);
            if (courseTimelineDetail != null)
            {
                _coursesTimelineDetail.Remove(courseTimelineDetail);
                await _context.SaveChangesAsync();
            }
        }
        // Phương thức để lấy CourseTimelineDetail theo CourseTimelineId
        public async Task<IEnumerable<CourseTimelineDetail>> GetByCourseTimelineIdAsync(Guid courseTimelineId)
        {
            return await _context.CourseTimelineDetails
                                 .Where(ct => ct.CourseTimelineId == courseTimelineId)
                                 .ToListAsync();
        }

    }
}
