using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Model;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly IClassRepository _classRepository;
        private readonly ICourseRepository _repository;

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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var course = await _repository.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return Ok(course);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Course course)
        {
            if (course == null)
            {
                return BadRequest("Course data is required.");
            }

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(course.CourseName) ||
                string.IsNullOrWhiteSpace(course.Content) ||
                course.Hours <= 0 ||
                course.Days <= 0 ||
                string.IsNullOrWhiteSpace(course.Category) ||
                course.Price <= 0 ||
                string.IsNullOrWhiteSpace(course.UserId)) // Kiểm tra UserId không bị thiếu
            {
                return BadRequest("Invalid course data. Please ensure all required fields are filled correctly.");
            }

            // Không cần thiết phải xử lý CourseTimelines ở đây
            await _repository.AddAsync(course);
            return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, Course course)
        {
            if (id != course.Id)
            {
                return BadRequest("Course ID mismatch.");
            }

            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(course.CourseName) ||
                string.IsNullOrWhiteSpace(course.Content) ||
                course.Hours <= 0 ||
                course.Days <= 0 ||
                string.IsNullOrWhiteSpace(course.Category) ||
                course.Price <= 0 ||
                string.IsNullOrWhiteSpace(course.UserId)) // Kiểm tra UserId không bị thiếu
            {
                return BadRequest("Invalid course data. Please ensure all required fields are filled correctly.");
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
        public async Task<ActionResult<ResponseDto>> GetByCourseId(Guid courseId)
        {
            var response = new ResponseDto();
            try
            {
                var classList = await _classRepository.GetByCourseIdAsync(courseId);
                if (classList == null || !classList.Any())
                {
                    response.IsSuccess = false;
                    response.Message = "No classes found for the specified course ID.";
                    return NotFound(response);
                }
                response.Result = classList;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("user/{userId}")] // Không cần thay đổi
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest("User ID is required.");
            }

            var courses = await _repository.GetAllByUserIdAsync(userId);
            if (courses == null || !courses.Any())
            {
                return NotFound("No courses found for the specified user ID.");
            }

            return Ok(courses);
        }
    }
}
