using Entity.Data;
using Entity.TeacherFolder;
using Google;
using IRepository;
using Microsoft.EntityFrameworkCore;
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

        public async Task<TeacherRating> CreateRatingAsync(TeacherRating rating)
        {
            // Thêm rating mới
            _context.TeacherRatings.Add(rating);
            await _context.SaveChangesAsync();

            // Tính trung bình sau khi thêm rating mới
            await UpdateAverageRating(rating.UserId);

            return rating;
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
    }
}
