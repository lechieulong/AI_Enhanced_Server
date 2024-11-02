using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model;
using Microsoft.AspNetCore.Authorization;
using Model.Utility;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _repository;
        private readonly IClassRepository _classRepository; // Keep or remove based on your need

        public CoursesController(ICourseRepository repository, IClassRepository classRepository)
        {
            _repository = repository;
            _classRepository = classRepository;
        }

        // GET: api/courses
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _repository.GetAllAsync();
            return Ok(courses.Select(course => new
            {
                course.Id,
                course.CourseName,
                course.Content,
                course.Hours,
                course.Days,
                Categories = course.Categories, // Return the list of categories
                course.Price,
                course.UserId,
                course.IsEnabled // Include IsEnabled status

            }));
        }

        // POST: api/courses
        [HttpPost]
        [Authorize(Roles = SD.Teacher)]
        public async Task<IActionResult> Create([FromBody] CourseDto courseDto) // Sử dụng CourseDto
        {
            if (courseDto == null || string.IsNullOrWhiteSpace(courseDto.CourseName) ||
                string.IsNullOrWhiteSpace(courseDto.Content) || courseDto.Hours <= 0 ||
                courseDto.Days <= 0 || courseDto.Categories == null ||
                !courseDto.Categories.Any() || // Ensure at least one category is provided
                courseDto.Price <= 0 || string.IsNullOrWhiteSpace(courseDto.UserId))
            {
                return BadRequest("Invalid course data.");
            }

            // Chuyển đổi từ CourseDto sang Course entity
            var course = new Course
            {
                Id = Guid.NewGuid(), // Tạo ID mới cho khóa học
                CourseName = courseDto.CourseName,
                Content = courseDto.Content,
                Hours = courseDto.Hours,
                Days = courseDto.Days,
                Categories = courseDto.Categories,
                Price = courseDto.Price,
                UserId = courseDto.UserId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsEnabled = true // Default IsEnabled to true if not specified
            };

            await _repository.CreateAsync(course);
            return Ok(course);
        }

        // PUT: api/courses/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Course course)
        {
            if (id != course.Id)
            {
                return BadRequest("Course ID mismatch.");
            }

            // You can also add validations for the Categories here if needed
            await _repository.UpdateAsync(course);
            return NoContent();
        }

        // DELETE: api/courses/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/courses/{courseId}/classes
        [HttpGet("{courseId:guid}/classes")]
        public async Task<IActionResult> GetClassesByCourseId(Guid courseId)
        {
            var classes = await _classRepository.GetByCourseIdAsync(courseId);
            return classes == null || !classes.Any() ? NotFound("No classes found.") : Ok(classes);
        }

        // GET: api/courses/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllCourseByUserId(string userId)
        {
            var courses = await _repository.GetAllCourseByUserIdAsync(userId);
            return courses == null || !courses.Any() ? NotFound("No courses found.") : Ok(courses);
        }

        // GET: api/courses/enabled
        [HttpGet("enabled")]
        public async Task<IActionResult> GetEnabledCourses()
        {
            var courses = await _repository.GetAllEnabledCoursesAsync();
            return courses == null || !courses.Any() ? NotFound("No enabled courses found.") : Ok(courses);
        }

        // GET: api/courses/disabled
        [HttpGet("disabled")]
        public async Task<IActionResult> GetDisabledCourses()
        {
            var courses = await _repository.GetAllDisabledCoursesAsync();
            return courses == null || !courses.Any() ? NotFound("No disabled courses found.") : Ok(courses);
        }

        // GET: api/courses/{id}/enabled
        [HttpGet("{id:guid}/enabled")]
        public async Task<IActionResult> IsCourseEnabled(Guid id)
        {
            var course = await _repository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            return Ok(new { course.Id, course.IsEnabled });
        }

        // PUT: api/courses/{courseId}/enabled
        [HttpPut("{courseId:guid}/enabled")]
        public async Task<IActionResult> UpdateCourseEnabledStatus(Guid courseId, [FromBody] bool isEnabled)
        {
            var course = await _repository.GetByIdAsync(courseId);
            if (course == null)
            {
                return NotFound("Course not found.");
            }

            await _repository.UpdateCourseEnabledStatusAsync(courseId, isEnabled);
            return Ok(new { courseId, IsEnabled = isEnabled });
        }
    }
}
