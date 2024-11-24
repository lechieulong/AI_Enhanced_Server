using System;
using System.Linq;
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

        [HttpPost("{courseId}/rate")]
        public async Task<IActionResult> RateCourse(Guid courseId, [FromBody] CourseRatingDto ratingDto)
        {
            if (ratingDto.RatingValue < 1 || ratingDto.RatingValue > 5)
                return BadRequest("Rating must be between 1 and 5.");

            bool isEnrolled = await _repository.UserHasEnrolledAsync(courseId, ratingDto.UserId);
            if (!isEnrolled)
                return BadRequest("User must be enrolled in the course to rate it.");

            bool hasRated = await _repository.UserHasRatedCourseAsync(courseId, ratingDto.UserId);
            if (hasRated)
                return BadRequest("User has already rated this course.");

            var rating = new CourseRating
            {
                CourseId = courseId,
                UserId = ratingDto.UserId,
                RatingValue = ratingDto.RatingValue,
                Review = ratingDto.Review,
                RatedAt = DateTime.Now
            };

            await _repository.AddRatingAsync(rating);
            return Ok(new { Message = "Rating added successfully." });
        }

        [HttpGet("{courseId}/ratings")]
        public async Task<IActionResult> GetCourseRatings(Guid courseId, [FromQuery] string userId = null)
        {
            var ratings = await _repository.GetCourseRatingsAsync(courseId);
            if (!string.IsNullOrEmpty(userId))
            {
                ratings = ratings.Where(r => r.UserId == userId).ToList();
            }

            var result = ratings.Select(r => new CourseRatingDto
            {
                UserId = r.UserId,
                RatingValue = r.RatingValue,
                Review = r.Review,
                RatedAt = r.RatedAt
            });

            return Ok(result);
        }

    }
}
