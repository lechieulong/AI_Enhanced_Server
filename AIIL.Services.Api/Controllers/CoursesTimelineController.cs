using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseTimelineController : ControllerBase
    {
        private readonly ICourseTimelineRepository _repository;

        public CourseTimelineController(ICourseTimelineRepository repository)
        {
            _repository = repository;
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
        public async Task<IActionResult> Create([FromBody] CourseTimeline courseTimeline)
        {
            if (courseTimeline == null)
            {
                return BadRequest("Timeline data is required.");
            }

            // Validate required fields
            if (courseTimeline.CourseId == Guid.Empty || // Assuming CourseId is a Guid
                string.IsNullOrWhiteSpace(courseTimeline.Title) ||
                string.IsNullOrWhiteSpace(courseTimeline.Description) ||
                string.IsNullOrWhiteSpace(courseTimeline.Author) ||
                courseTimeline.EventDate == default)
            {
                return BadRequest("Invalid timeline data.");
            }

            // Check if CourseId exists
            bool courseExists = await _repository.CheckExistCourseIdAsync(courseTimeline.CourseId);
            if (!courseExists)
            {
                return BadRequest("Invalid CourseId.");
            }

            // Add CourseTimeline to the database
            await _repository.AddAsync(courseTimeline);
            return CreatedAtAction(nameof(GetById), new { id = courseTimeline.Id }, courseTimeline);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourseTimeline courseTimeline)
        {
            if (id != courseTimeline.Id)
            {
                return BadRequest("ID mismatch.");
            }

            // Check if the course timeline exists before updating
            var existingTimeline = await _repository.GetByIdAsync(id);
            if (existingTimeline == null)
            {
                return NotFound();
            }

            await _repository.UpdateAsync(courseTimeline);
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
                EventDateFormatted = ct.EventDate.ToString("dd/MM/yyyy"),
                ct.Title,
                ct.Description,
                ct.Author
            }).ToList();

            return Ok(courseTimelinesFormatted);
        }
    }
}
