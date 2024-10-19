using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entity;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseTimelineDetailController : ControllerBase
    {
        private readonly ICourseTimelineDetailRepository _repository;

        public CourseTimelineDetailController(ICourseTimelineDetailRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courseTimelineDetail = await _repository.GetAllAsync();
            return Ok(courseTimelineDetail);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var courseTimelineDetail = await _repository.GetByIdAsync(id);
            if (courseTimelineDetail == null)
            {
                return NotFound();
            }
            return Ok(courseTimelineDetail);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CourseTimelineDetail courseTimelineDetail)
        {
            await _repository.CreateAsync(courseTimelineDetail);
            return CreatedAtAction(nameof(GetById), new { id = courseTimelineDetail.Id }, courseTimelineDetail);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, CourseTimelineDetail courseTimelineDetail)
        {
            if (id != courseTimelineDetail.Id)
            {
                return BadRequest();
            }

            await _repository.UpdateAsync(courseTimelineDetail);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("CourseTimeline/{courseTimelineId}")]
        public async Task<IActionResult> GetByCourseTimelineId(Guid courseTimelineId)
        {
            var courseTimelineDetails = await _repository.GetByCourseTimelineIdAsync(courseTimelineId);

            if (courseTimelineDetails == null || !courseTimelineDetails.Any())
            {
                return NotFound(new { message = "Không tìm thấy chi tiết nào cho CourseTimelineId đã cho." });
            }

            return Ok(courseTimelineDetails);
        }
    }
}
