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
    public class CourseRatingRepository : ICourseRatingRepository
    {
        private readonly AppDbContext _context;

        public CourseRatingRepository(AppDbContext context)
        {
            _context = context;
        }

        // Kiểm tra nếu người dùng đã đăng ký khóa học
        public async Task<bool> UserHasEnrolledAsync(Guid courseId, string userId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
        }

        // Kiểm tra nếu người dùng đã đánh giá khóa học
        public async Task<bool> UserHasRatedCourseAsync(Guid courseId, string userId)
        {
            return await _context.CourseRatings
                .AnyAsync(r => r.CourseId == courseId && r.UserId == userId);
        }

        // Thêm một đánh giá mới
        public async Task AddRatingAsync(CourseRating rating)
        {
            _context.CourseRatings.Add(rating);
            await _context.SaveChangesAsync();
        }

        // Lấy danh sách đánh giá của một khóa học
        public async Task<IEnumerable<CourseRating>> GetCourseRatingsAsync(Guid courseId)
        {
            return await _context.CourseRatings
                .Where(r => r.CourseId == courseId)
                .ToListAsync();
        }
    }
}
