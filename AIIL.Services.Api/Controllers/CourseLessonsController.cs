using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;
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
        public async Task<ActionResult<CourseLesson>> Create(CourseLesson courseLesson)
        {
            if (courseLesson == null)
                return BadRequest();

            var createdCourseLesson = await _courseLessonRepository.AddAsync(courseLesson);
            return CreatedAtAction(nameof(GetById), new { id = createdCourseLesson.Id }, createdCourseLesson);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CourseLesson courseLesson)
        {
            if (id != courseLesson.Id)
                return BadRequest();

            var existingCourseLesson = await _courseLessonRepository.GetByIdAsync(id);
            if (existingCourseLesson == null)
                return NotFound();

            await _courseLessonRepository.UpdateAsync(courseLesson);
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
    }
}
