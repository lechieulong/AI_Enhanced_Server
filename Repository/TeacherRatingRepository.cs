using Entity;
using Entity.CourseFolder;
using Entity.Data;
using Entity.TeacherFolder;
using Google;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TeacherRatingRepository : ITeacherRatingRepository
    {
        private readonly AppDbContext _context;

        public TeacherRatingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateRatingAsync(TeacherRating rating)
        {

            _context.TeacherRatings.Add(rating);
            await _context.SaveChangesAsync();

            await UpdateAverageRating(rating.UserId);
        }

        public async Task<IEnumerable<TeacherRating>> GetAllRatingsAsync()
        {
            return await _context.TeacherRatings.ToListAsync();
        }

        private async Task UpdateAverageRating(string userId)
        {
            // Lấy tất cả các đánh giá của user cụ thể
            var ratings = await _context.TeacherRatings
                .Where(r => r.UserId == userId)
                .ToListAsync();

            if (ratings.Any())
            {
                var average = ratings.Average(r => r.RatingValue);

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    user.AverageRating = average;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
            }
        }
        public async Task<IEnumerable<TopRatedTeacherDto>> GetTopRatedTeachersAsync()
        {
            var topTeachers = await (
                from user in _context.ApplicationUsers
                join userRole in _context.UserRoles on user.Id equals userRole.UserId
                join role in _context.Roles on userRole.RoleId equals role.Id
                where role.Name == "TEACHER" // Lọc người dùng có vai trò là TEACHER
                && user.RatingCount >= 0 // Đảm bảo người dùng có đánh giá
                && user.UserName != "aienhancedieltsprep" // Loại bỏ người dùng có username này
                orderby user.RatingCount descending, user.AverageRating descending // Sắp xếp theo RatingCount và AverageRating
                select new TopRatedTeacherDto
                {
                    UserName = user.UserName,
                    ImageURL = user.ImageURL,
                    AverageRating = user.AverageRating,
                    RatingCount = user.RatingCount
                }
            ).Take(8) // Giới hạn kết quả trả về chỉ 8 người
            .ToListAsync();

            return topTeachers;
        }
        public async Task<bool> HasLearnerRatedTeacherAsync(string userId, string learnerId)
        {
            return await Task.FromResult(
                _context.TeacherRatings.Any(r => r.UserId == userId && r.LearnerID == learnerId)
            );
        }
    }
}
