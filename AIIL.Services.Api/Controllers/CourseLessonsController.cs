using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;
using IRepository;
using Model;
using Repository;
using Model.Course;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseLessonsController : ControllerBase
    {
        private readonly ICourseLessonRepository _courseLessonRepository;
        private readonly ITestExamRepository _testExamRepository;
        public CourseLessonsController(ICourseLessonRepository courseLessonRepository, ITestExamRepository testExamRepository)
        {
            _courseLessonRepository = courseLessonRepository;
            _testExamRepository = testExamRepository;
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
        public async Task<ActionResult<CourseLesson>> Create([FromBody] Model.CourseLessonDto courseLessonDto)
        {
            if (courseLessonDto == null || courseLessonDto.CoursePartId == Guid.Empty || string.IsNullOrWhiteSpace(courseLessonDto.Title))
            {
                return BadRequest("Invalid course lesson data.");
            }
            var maxOrder = await _courseLessonRepository.GetMaxOrderByCoursePartIdAsync(courseLessonDto.CoursePartId);
            var courseLesson = new CourseLesson
            {
                Id = Guid.NewGuid(),
                CoursePartId = courseLessonDto.CoursePartId,
                Title = courseLessonDto.Title,
                Order = maxOrder + 1
            };

            var createdCourseLesson = await _courseLessonRepository.AddAsync(courseLesson);
            return CreatedAtAction(nameof(GetById), new { id = createdCourseLesson.Id }, createdCourseLesson);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Model.CourseLessonDto courseLessonDto)
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
        [HttpGet("GetTestExamByLessonId/{lessonId}")]
        public async Task<IActionResult> GetTestExamByLessonId(Guid lessonId)
        {
            var testExam = await _testExamRepository.GetTestExamByLessonIdAsync(lessonId);
            if (testExam == null)
            {
                return NotFound("TestExam not found for the given LessonId.");
            }

            return Ok(testExam);
        }
    }
}
