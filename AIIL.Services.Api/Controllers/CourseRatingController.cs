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
            bool isEnrolled = await _repository.UserHasEnrolledAsync(courseId, ratingDto.UserId);
            if (!isEnrolled)
            {
                return BadRequest("User must be enrolled in the course to rate it.");
            }

            bool hasRated = await _repository.UserHasRatedCourseAsync(courseId, ratingDto.UserId);
            if (hasRated)
            {
                return BadRequest("User has already rated this course.");
            }

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

        [HttpGet("{courseId}/ratings")]
        public async Task<IActionResult> GetCourseRatings(Guid courseId, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("UserId is required.");
            }

            var ratings = await _repository.GetCourseRatingsAsync(courseId);
            ratings = ratings.Where(r => r.UserId.ToString() == userId).ToList();

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
