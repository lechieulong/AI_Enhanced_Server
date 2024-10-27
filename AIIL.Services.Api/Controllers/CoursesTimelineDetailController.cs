using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Model; // Thêm không gian tên cho DTO
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            var courseTimelineDetails = await _repository.GetAllAsync();
            // Chuyển đổi sang DTO nếu cần
            var courseTimelineDetailsDto = courseTimelineDetails.Select(ct => new CourseTimelineDetailDto
            {
                Id = ct.Id,
                CourseTimelineId = ct.CourseTimelineId,
                Title = ct.Title,
                VideoUrl = ct.VideoUrl,
                Topic = ct.Topic,
                IsEnabled = ct.IsEnabled // Thêm IsEnabled vào DTO
            });

            return Ok(courseTimelineDetailsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var courseTimelineDetail = await _repository.GetByIdAsync(id);
            if (courseTimelineDetail == null)
            {
                return NotFound();
            }

            // Chuyển đổi sang DTO
            var courseTimelineDetailDto = new CourseTimelineDetailDto
            {
                Id = courseTimelineDetail.Id,
                CourseTimelineId = courseTimelineDetail.CourseTimelineId,
                Title = courseTimelineDetail.Title,
                VideoUrl = courseTimelineDetail.VideoUrl,
                Topic = courseTimelineDetail.Topic,
                IsEnabled = courseTimelineDetail.IsEnabled // Thêm IsEnabled vào DTO
            };

            return Ok(courseTimelineDetailDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CourseTimelineDetailDto courseTimelineDetailDto)
        {
            var courseTimelineDetail = new CourseTimelineDetailDto
            {
                Id = Guid.NewGuid(),
                CourseTimelineId = courseTimelineDetailDto.CourseTimelineId,
                Title = courseTimelineDetailDto.Title,
                VideoUrl = courseTimelineDetailDto.VideoUrl,
                Topic = courseTimelineDetailDto.Topic,
                IsEnabled = true // Mặc định IsEnabled là true khi tạo mới
            };

            await _repository.CreateAsync(courseTimelineDetail);
            return CreatedAtAction(nameof(GetById), new { id = courseTimelineDetail.Id }, courseTimelineDetailDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, CourseTimelineDetailDto courseTimelineDetailDto)
        {
            if (id != courseTimelineDetailDto.Id)
            {
                return BadRequest();
            }

            var courseTimelineDetail = new CourseTimelineDetailDto
            {
                Id = courseTimelineDetailDto.Id,
                CourseTimelineId = courseTimelineDetailDto.CourseTimelineId,
                Title = courseTimelineDetailDto.Title,
                VideoUrl = courseTimelineDetailDto.VideoUrl,
                Topic = courseTimelineDetailDto.Topic,
                IsEnabled = courseTimelineDetailDto.IsEnabled // Cập nhật IsEnabled
            };

            await _repository.UpdateAsync(courseTimelineDetail);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("CourseTimelines/Details")]
        public async Task<IActionResult> GetByCourseTimelineIds([FromQuery(Name = "courseTimelineIds")] IEnumerable<Guid> courseTimelineIds)
        {
            // Kiểm tra nếu courseTimelineIds null hoặc rỗng
            if (courseTimelineIds == null || !courseTimelineIds.Any())
            {
                return BadRequest(new { message = "Danh sách CourseTimelineId không được để trống." });
            }

            // Lấy thông tin chi tiết theo danh sách CourseTimelineId
            var courseTimelineDetailsList = await _repository.GetByCourseTimelineIdsAsync(courseTimelineIds.ToList());

            // Kiểm tra nếu không tìm thấy thông tin chi tiết nào
            if (courseTimelineDetailsList == null || !courseTimelineDetailsList.Any())
            {
                return NotFound(new { message = "Không tìm thấy chi tiết nào cho các CourseTimelineId đã cho." });
            }

            // Trả về danh sách chi tiết nếu tìm thấy
            return Ok(courseTimelineDetailsList);
        }
    }
}
