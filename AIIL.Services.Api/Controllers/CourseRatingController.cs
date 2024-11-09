using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Entity.CourseFolder;
using Model;
using IRepository;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseRatingController : ControllerBase
    {
        private readonly ICourseRatingRepository _repository;

        public CourseRatingController(ICourseRatingRepository repository)
        {
            _repository = repository;
        }

        // API để thêm đánh giá khóa học
        [HttpPost("{courseId}/rate")]
        public async Task<IActionResult> RateCourse(Guid courseId, [FromBody] CourseRatingDto ratingDto)
        {
            // Kiểm tra người dùng đã đăng ký khóa học chưa
            bool isEnrolled = await _repository.UserHasEnrolledAsync(courseId, ratingDto.UserId);
            if (!isEnrolled)
            {
                return BadRequest("User must be enrolled in the course to rate it.");
            }

            // Kiểm tra người dùng đã đánh giá khóa học này chưa
            bool hasRated = await _repository.UserHasRatedCourseAsync(courseId, ratingDto.UserId);
            if (hasRated)
            {
                return BadRequest("User has already rated this course.");
            }

            // Tạo và lưu đánh giá mới
            var rating = new CourseRating
            {
                CourseId = courseId,
                UserId = ratingDto.UserId,
                RatingValue = ratingDto.RatingValue,
                Review = ratingDto.Review
            };

            await _repository.AddRatingAsync(rating);
            return Ok("Rating added successfully.");
        }

        // API để lấy danh sách đánh giá của khóa học
        [HttpGet("{courseId}/ratings")]
        public async Task<IActionResult> GetCourseRatings(Guid courseId)
        {
            var ratings = await _repository.GetCourseRatingsAsync(courseId);
            var result = ratings.Select(r => new
            {
                r.UserId,
                r.RatingValue,
                r.Review,
                r.RatedAt
            });
            return Ok(result);
        }
    }
}
