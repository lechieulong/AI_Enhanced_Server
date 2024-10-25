using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Model;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _repository;
        private readonly IClassRepository _classRepository;

        public CoursesController(ICourseRepository repository, IClassRepository classRepository)
        {
            _repository = repository;
            _classRepository = classRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _repository.GetAllAsync();
            return Ok(courses);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Course course)
        {
            if (course == null || string.IsNullOrWhiteSpace(course.CourseName) ||
                string.IsNullOrWhiteSpace(course.Content) || course.Hours <= 0 ||
                course.Days <= 0 || string.IsNullOrWhiteSpace(course.Category) ||
                course.Price <= 0 || string.IsNullOrWhiteSpace(course.UserId))
            {
                return BadRequest("Invalid course data.");
            }

            // Mặc định IsEnabled là true nếu không được chỉ định
            course.IsEnabled = true;

            await _repository.CreateAsync(course);
            return Ok(course);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Course course)
        {
            if (id != course.Id)
            {
                return BadRequest("Course ID mismatch.");
            }

            await _repository.UpdateAsync(course);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{courseId:guid}/classes")]
        public async Task<IActionResult> GetClassesByCourseId(Guid courseId)
        {
            var classes = await _classRepository.GetByCourseIdAsync(courseId);
            return classes == null || !classes.Any() ? NotFound("No classes found.") : Ok(classes);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllCourseByUserId(string userId)
        {
            var courses = await _repository.GetAllCourseByUserIdAsync(userId);
            return courses == null || !courses.Any() ? NotFound("No courses found.") : Ok(courses);
        }

        // Thêm phương thức để lấy tất cả khóa học với trạng thái IsEnabled
        [HttpGet("enabled")]
        public async Task<IActionResult> GetEnabledCourses()
        {
            var courses = await _repository.GetAllEnabledCoursesAsync();
            return courses == null || !courses.Any() ? NotFound("No enabled courses found.") : Ok(courses);
        }

        // Thêm phương thức để lấy tất cả khóa học với trạng thái IsDisabled
        [HttpGet("disabled")]
        public async Task<IActionResult> GetDisabledCourses()
        {
            var courses = await _repository.GetAllDisabledCoursesAsync();
            return courses == null || !courses.Any() ? NotFound("No disabled courses found.") : Ok(courses);
        }
    }
}
