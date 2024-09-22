﻿using IRepository;
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
        public async Task<IActionResult> GetById(int id)
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

            // Kiểm tra các trường bắt buộc
            if (courseTimeline.CourseId <= 0 ||
                string.IsNullOrWhiteSpace(courseTimeline.Title) ||
                string.IsNullOrWhiteSpace(courseTimeline.Description) ||
                string.IsNullOrWhiteSpace(courseTimeline.Author) ||
                courseTimeline.EventDate == default)
            {
                return BadRequest("Invalid timeline data.");
            }

            // Kiểm tra xem CourseId có tồn tại không
            bool courseExists = await _repository.CheckExistCourseIdAsync(courseTimeline.CourseId);
            if (!courseExists)
            {
                return BadRequest("Invalid CourseId.");
            }

            // Thêm CourseTimeline vào cơ sở dữ liệu
            await _repository.AddAsync(courseTimeline);
            return CreatedAtAction(nameof(GetById), new { id = courseTimeline.Id }, courseTimeline);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CourseTimeline courseTimeline)
        {
            if (id != courseTimeline.Id)
            {
                return BadRequest();
            }

            await _repository.UpdateAsync(courseTimeline);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("Course/{courseId}")]
        public async Task<IActionResult> GetByCourseId(int courseId)
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
