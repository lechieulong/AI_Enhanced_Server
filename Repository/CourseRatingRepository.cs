using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entity.CourseFolder;
using IRepository;
using Entity.Data;
using Model;

namespace Repository
{
    public class CourseRatingRepository : ICourseRatingRepository
    {
        private readonly AppDbContext _context;

        public CourseRatingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> UserHasEnrolledAsync(Guid courseId, string userId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.CourseId == courseId && e.UserId == userId);
        }

        public async Task<bool> UserHasRatedCourseAsync(Guid courseId, string userId)
        {
            return await _context.CourseRatings
                .AnyAsync(r => r.CourseId == courseId && r.UserId == userId);
        }

        public async Task AddRatingAsync(CourseRating rating)
        {
            _context.CourseRatings.Add(rating);
            await _context.SaveChangesAsync();

            // Tự động cập nhật trung bình cộng rating
            await UpdateCourseRatingAsync(rating.CourseId);
        }

        public async Task<List<CourseRating>> GetCourseRatingsAsync(Guid courseId)
        {
            return await _context.CourseRatings
                .Where(r => r.CourseId == courseId)
                .ToListAsync();
        }

        private async Task UpdateCourseRatingAsync(Guid courseId)
        {
            var course = await _context.Courses
                .Include(c => c.CourseRatings)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course != null)
            {
                var ratings = course.CourseRatings;

                if (ratings.Any())
                {
                    course.AverageRating = Math.Round(ratings.Average(r => r.RatingValue), 2);
                    course.RatingCount = ratings.Count;
                }
                else
                {
                    course.AverageRating = 0;
                    course.RatingCount = 0;
                }

                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<CourseRatingWithUserInfo>> GetCourseRatingsWithUserInfoAsync(Guid courseId)
        {
            var courseRatings = await _context.CourseRatings
                .Where(cr => cr.CourseId == courseId)
                .Include(cr => cr.User) // Ensure User is included in the query
                .ToListAsync();

            return courseRatings.Select(cr => new CourseRatingWithUserInfo
            {
                RatingValue = cr.RatingValue,
                Review = cr.Review,
                RatedAt = cr.RatedAt.ToString("yyyy/MM/dd"), // Format the date as required
                Username = cr.User.UserName,  // Mapping the User's username
                ImageUrl = cr.User.ImageURL  // Mapping the User's imageUrl
            }).ToList();
        }


    }
}
