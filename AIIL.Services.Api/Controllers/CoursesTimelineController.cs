using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Model; // Đừng quên thêm namespace cho DTO
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseTimelineController : ControllerBase
    {
        private readonly ICourseTimelineRepository _repository;
        private readonly ICourseRepository _courseRepository; // Thêm repository để lấy thông tin khóa học
        private readonly IUserRepository _userRepository; // Thêm repository để lấy thông tin người dùng

        public CourseTimelineController(ICourseTimelineRepository repository, ICourseRepository courseRepository, IUserRepository userRepository)
        {
            _repository = repository;
            _courseRepository = courseRepository;
            _userRepository = userRepository; // Khởi tạo user repository
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
                // Validate required fields
                if (timelineDto.CourseId == Guid.Empty || // Assuming CourseId is a Guid
                    string.IsNullOrWhiteSpace(timelineDto.Title) ||
                    string.IsNullOrWhiteSpace(timelineDto.Description) ||
                    timelineDto.EventDate == default)
                {
                    return BadRequest("Invalid timeline data.");
                }

                // Check if CourseId exists
                var course = await _courseRepository.GetByIdAsync(timelineDto.CourseId);
                if (course == null)
                {
                    return BadRequest($"Invalid CourseId: {timelineDto.CourseId}.");
                }

                // Lấy thông tin người dùng từ UserId của khóa học
                var user = await _userRepository.GetUserByIdAsync(course.UserId);
                timelineDto.Author = user?.Name ?? "Unknown"; // Gán tên người dùng vào Author

                // Map DTO to Entity
                var courseTimeline = new CourseTimeline
                {
                    CourseId = timelineDto.CourseId,
                    EventDate = timelineDto.EventDate,
                    Title = timelineDto.Title,
                    Description = timelineDto.Description,
                    Author = timelineDto.Author,
                    IsEnabled = timelineDto.IsEnabled // Gán IsEnabled từ DTO
                };

                // Add CourseTimeline to the list of added timelines
                addedTimelines.Add(courseTimeline);
            }

            // Add all valid CourseTimelines to the database
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

            // Check if the course timeline exists before updating
            var existingTimeline = await _repository.GetByIdAsync(id);
            if (existingTimeline == null)
            {
                return NotFound();
            }

            // Map DTO to Entity for update
            existingTimeline.EventDate = timelineDto.EventDate;
            existingTimeline.Title = timelineDto.Title;
            existingTimeline.Description = timelineDto.Description;
            existingTimeline.Author = timelineDto.Author;
            existingTimeline.IsEnabled = timelineDto.IsEnabled; // Cập nhật IsEnabled

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
                EventDateFormatted = ct.EventDate.ToString("dd/MM/yyyy"),
                ct.Title,
                ct.Description,
                ct.Author,
                ct.IsEnabled // Thêm IsEnabled vào phản hồi
            }).ToList();

            return Ok(courseTimelinesFormatted);
        }

        [HttpPut("{id}/enabled")] // API endpoint cho việc cập nhật IsEnabled
        public async Task<IActionResult> UpdateCourseTimelineEnabledStatus(Guid id, [FromBody] bool isEnabled)
        {
            // Kiểm tra xem timeline có tồn tại không
            var existingTimeline = await _repository.GetByIdAsync(id);
            if (existingTimeline == null)
            {
                return NotFound("CourseTimeline not found.");
            }

            // Cập nhật thuộc tính IsEnabled
            existingTimeline.IsEnabled = isEnabled;

            // Cập nhật trong cơ sở dữ liệu
            await _repository.UpdateAsync(existingTimeline);

            return Ok(new { id, IsEnabled = existingTimeline.IsEnabled }); // Trả về ID và trạng thái mới
        }
        // Trong Auth/Controllers/CoursesController.cs
        // GET: api/courses/{courseId}/timelines
        [HttpGet("{courseId:guid}/timelines")]
        public async Task<IActionResult> GetTimelinesByCourseId(Guid courseId)
        {
            var timelines = await _repository.GetCourseTimelinesByCourseIdAsync(courseId);
            return timelines == null || !timelines.Any()
                ? NotFound("No timelines found for this course.")
                : Ok(timelines.Select(t => new
                {
                    t.Id, // CourseTimelineId
                    t.Title // Title
                }));
        }


    }
}