using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;
using IRepository;
using Model;
using Repository;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseLessonsController : ControllerBase
    {
        private readonly ICourseLessonRepository _courseLessonRepository;

        public CourseLessonsController(ICourseLessonRepository courseLessonRepository)
        {
            _courseLessonRepository = courseLessonRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseLesson>>> GetAll()
        {
            var courseLessons = await _courseLessonRepository.GetAllAsync();
            return Ok(courseLessons);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseLesson>> GetById(Guid id)
        {
            var courseLesson = await _courseLessonRepository.GetByIdAsync(id);
            if (courseLesson == null)
                return NotFound();

            return Ok(courseLesson);
        }

        [HttpPost]
        public async Task<ActionResult<CourseLesson>> Create([FromBody] CourseLessonDto courseLessonDto)
        {
            if (courseLessonDto == null || courseLessonDto.CoursePartId == Guid.Empty || string.IsNullOrWhiteSpace(courseLessonDto.Title))
            {
                return BadRequest("Invalid course lesson data.");
            }

            var courseLesson = new CourseLesson
            {
                Id = Guid.NewGuid(),
                CoursePartId = courseLessonDto.CoursePartId,
                Title = courseLessonDto.Title
            };

            var createdCourseLesson = await _courseLessonRepository.AddAsync(courseLesson);
            return CreatedAtAction(nameof(GetById), new { id = createdCourseLesson.Id }, createdCourseLesson);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CourseLessonDto courseLessonDto)
        {
            if (courseLessonDto == null || id == Guid.Empty || string.IsNullOrWhiteSpace(courseLessonDto.Title))
            {
                return BadRequest("Invalid course lesson data.");
            }

            var existingCourseLesson = await _courseLessonRepository.GetByIdAsync(id);
            if (existingCourseLesson == null)
                return NotFound();

            existingCourseLesson.CoursePartId = courseLessonDto.CoursePartId;
            existingCourseLesson.Title = courseLessonDto.Title;

            await _courseLessonRepository.UpdateAsync(existingCourseLesson);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _courseLessonRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        [HttpGet("CoursePart/{coursePartId}")]
        public async Task<ActionResult<IEnumerable<CourseLesson>>> GetByCoursePartId(Guid coursePartId)
        {
            var courseLessons = await _courseLessonRepository.GetByCoursePartIdAsync(coursePartId);
            if (courseLessons == null || !courseLessons.Any())
                return NotFound("No lessons found for the specified CoursePartId.");

            return Ok(new
            {
                CourseLessons = courseLessons,
            });
        }

    }
}
