using AutoMapper;
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
                UserId = dto.UserId,
                LearnerID = dto.LearnerID,
                RatingValue = dto.RatingValue,
                Review = dto.Review,
                RatedAt = dto.RatedAt,
            };

            var createdRating = await _repository.CreateRatingAsync(rating);

            var createdRatingDto = new TeacherRatingDto
            {
                UserId = createdRating.UserId,
                LearnerID = createdRating.LearnerID,
                RatingValue = createdRating.RatingValue,
                Review = createdRating.Review,
                RatedAt = createdRating.RatedAt,
            };

            return CreatedAtAction(nameof(GetAllRatings), new { id = createdRating.Id }, createdRatingDto);
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
    }
}
