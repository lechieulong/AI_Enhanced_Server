using AutoMapper;
using Entity;
using Entity.Data;
using Entity.TeacherFolder;
using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherRatingsController : ControllerBase
    {
        private readonly ITeacherRatingRepository _repository;
        private readonly AppDbContext _context;

        public TeacherRatingsController(ITeacherRatingRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<TeacherRatingDto>> CreateRating([FromBody] TeacherRatingDto dto)
        {
            var rating = new TeacherRating
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                LearnerID = dto.LearnerID,
                RatingValue = dto.RatingValue,
                Review = dto.Review,
                RatedAt = dto.RatedAt,
            };

            await _repository.CreateRatingAsync(rating);
            return Ok(new { Message = "Rating added successfully." });
        }


        [HttpGet("{userId}/hotTeacher")]
        public IActionResult GetStatistics(string userId)
        {
            var ratings = _context.TeacherRatings.Where(r => r.UserId == userId);

            var uniqueLearners = ratings.Select(r => r.LearnerID).Distinct().Count();
            var averageRating = ratings.Any()
                ? ratings.Average(r => r.RatingValue)
                : 0;

            return Ok(new
            {
                UniqueLearners = uniqueLearners,
                AverageRating = averageRating
            });
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherRatingDto>>> GetAllRatings()
        {
            var ratings = await _repository.GetAllRatingsAsync();

            var ratingsDto = ratings.Select(r => new TeacherRatingDto
            {
                UserId = r.UserId,
                LearnerID = r.LearnerID,
                RatingValue = r.RatingValue,
                Review = r.Review,
                RatedAt = r.RatedAt
            });

            return Ok(ratingsDto);
        }
        [HttpGet("TopRatedTeachers")]
        public async Task<ActionResult<IEnumerable<TopRatedTeacherDto>>> GetTopRatedTeachers()
        {
            var topTeachers = await _repository.GetTopRatedTeachersAsync();
            return Ok(topTeachers);
        }


    }
}
