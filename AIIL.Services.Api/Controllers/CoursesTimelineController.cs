using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Model;
using System.Linq;
using System.Threading.Tasks;
using Entity.CourseFolder;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseTimelineController : ControllerBase
    {
        private readonly ICourseTimelineRepository _repository;
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;

        public CourseTimelineController(ICourseTimelineRepository repository, ICourseRepository courseRepository, IUserRepository userRepository)
        {
            _repository = repository;
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courseTimelines = await _repository.GetAllAsync();
            return Ok(courseTimelines);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var courseTimeline = await _repository.GetByIdAsync(id);
            if (courseTimeline == null)
            {
                return NotFound();
            }
            return Ok(courseTimeline);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] List<CourseTimelineDto> courseTimelineDtos)
        {
            if (courseTimelineDtos == null || !courseTimelineDtos.Any())
            {
                return BadRequest("At least one timeline is required.");
            }

            var addedTimelines = new List<CourseTimeline>();

            foreach (var timelineDto in courseTimelineDtos)
            {
                if (timelineDto.CourseId == Guid.Empty ||
                    string.IsNullOrWhiteSpace(timelineDto.Title) ||
                    string.IsNullOrWhiteSpace(timelineDto.Description))
                {
                    return BadRequest("Invalid timeline data.");
                }

                var course = await _courseRepository.GetByIdAsync(timelineDto.CourseId);
                if (course == null)
                {
                    return BadRequest($"Invalid CourseId: {timelineDto.CourseId}.");
                }

                var user = await _userRepository.GetUserByIdAsync(course.UserId);
                timelineDto.Author = user?.Name ?? "Unknown";

                var courseTimeline = new CourseTimeline
                {
                    CourseId = timelineDto.CourseId,
                    Title = timelineDto.Title,
                    Description = timelineDto.Description,
                    Author = timelineDto.Author,
                    IsEnabled = timelineDto.IsEnabled
                };

                addedTimelines.Add(courseTimeline);
            }

            foreach (var timeline in addedTimelines)
            {
                await _repository.CreateAsync(timeline);
            }

            return CreatedAtAction(nameof(GetAll), addedTimelines);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourseTimelineDto timelineDto)
        {
            if (id != timelineDto.CourseId)
            {
                return BadRequest("ID mismatch.");
            }

            var existingTimeline = await _repository.GetByIdAsync(id);
            if (existingTimeline == null)
            {
                return NotFound();
            }

            existingTimeline.Order = timelineDto.Order;
            existingTimeline.Title = timelineDto.Title;
            existingTimeline.Description = timelineDto.Description;
            existingTimeline.Author = timelineDto.Author;
            existingTimeline.IsEnabled = timelineDto.IsEnabled;

            await _repository.UpdateAsync(existingTimeline);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var courseTimelineExists = await _repository.GetByIdAsync(id);
            if (courseTimelineExists == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("Course/{courseId}")]
        public async Task<IActionResult> GetByCourseId(Guid courseId)
        {
            var courseTimelines = await _repository.GetByCourseIdAsync(courseId);
            if (courseTimelines == null || !courseTimelines.Any())
            {
                return NotFound(new { message = "No timelines found for this course" });
            }

            var courseTimelinesFormatted = courseTimelines.Select(ct => new
            {
                ct.Id,
                ct.CourseId,
                ct.Order,
                ct.Title,
                ct.Description,
                ct.Author,
                ct.IsEnabled
            }).ToList();

            return Ok(courseTimelinesFormatted);
        }

        [HttpPut("{id}/enabled")]
        public async Task<IActionResult> UpdateCourseTimelineEnabledStatus(Guid id, [FromBody] bool isEnabled)
        {
            var existingTimeline = await _repository.GetByIdAsync(id);
            if (existingTimeline == null)
            {
                return NotFound("CourseTimeline not found.");
            }

            existingTimeline.IsEnabled = isEnabled;
            await _repository.UpdateAsync(existingTimeline);

            return Ok(new { id, IsEnabled = existingTimeline.IsEnabled });
        }

        [HttpGet("{courseId:guid}/timelines")]
        public async Task<IActionResult> GetTimelinesByCourseId(Guid courseId)
        {
            var timelines = await _repository.GetCourseTimelinesByCourseIdAsync(courseId);
            return timelines == null || !timelines.Any()
                ? NotFound("No timelines found for this course.")
                : Ok(timelines.Select(t => new
                {
                    t.Id,
                    t.Title
                }));
        }
    }
}
