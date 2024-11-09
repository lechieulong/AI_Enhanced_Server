using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entity;
using Model; 
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Repository;
using Entity.CourseFolder;
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
            // Chuyển đổi sang DTO
            var courseTimelineDetailsDto = courseTimelineDetails.Select(ct => new CourseTimelineDetailDto
            {
                CourseTimelineId = ct.CourseTimelineId,
                Title = ct.Title,
                VideoUrl = ct.VideoUrl,
                Topic = ct.Topic,
                IsEnabled = ct.IsEnabled // Thêm IsEnabled vào DTO
            }).ToList(); // Chuyển đổi thành danh sách

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
                CourseTimelineId = courseTimelineDetail.CourseTimelineId,
                Title = courseTimelineDetail.Title,
                VideoUrl = courseTimelineDetail.VideoUrl,
                Topic = courseTimelineDetail.Topic,
                IsEnabled = courseTimelineDetail.IsEnabled // Thêm IsEnabled vào DTO
            };

            return Ok(courseTimelineDetailDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] List<CourseTimelineDetailDto> courseTimelineDetailDtos)
        {
            if (courseTimelineDetailDtos == null || !courseTimelineDetailDtos.Any())
            {
                return BadRequest("At least one timeline detail is required.");
            }

            var addedTimelineDetails = new List<CourseTimelineDetail>();

            foreach (var timelineDetailDto in courseTimelineDetailDtos)
            {
                // Validate required fields
                if (timelineDetailDto.CourseTimelineId == Guid.Empty ||
                    string.IsNullOrWhiteSpace(timelineDetailDto.Title) ||
                    string.IsNullOrWhiteSpace(timelineDetailDto.Topic))
                {
                    return BadRequest("Invalid timeline detail data.");
                }

                // Map DTO to Entity
                var courseTimelineDetail = new CourseTimelineDetail
                {
                    CourseTimelineId = timelineDetailDto.CourseTimelineId,
                    Title = timelineDetailDto.Title,
                    VideoUrl = timelineDetailDto.VideoUrl,
                    Topic = timelineDetailDto.Topic,
                    IsEnabled = timelineDetailDto.IsEnabled // Gán IsEnabled từ DTO
                };
                // Thêm CourseTimelineDetail vào cơ sở dữ liệu
                addedTimelineDetails.Add(courseTimelineDetail);
            }
            foreach (var timelineDetail in addedTimelineDetails)
            {
                await _repository.CreateAsync(timelineDetail);
            }
            return CreatedAtAction(nameof(GetAll), addedTimelineDetails);
        }




        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateAsync(Guid id, CourseTimelineDetailDto courseTimelineDetailDto)
        //{
        //    // Kiểm tra nếu Id trong DTO không khớp với Id trong URL
        //    if (id != courseTimelineDetailDto.Id)
        //    {
        //        return BadRequest("Id in URL and Id in body must match.");
        //    }

        //    var courseTimelineDetail = new CourseTimelineDetailDto
        //    {
        //        Id = id, // Gán Id từ URL cho đối tượng
        //        CourseTimelineId = courseTimelineDetailDto.CourseTimelineId,
        //        Title = courseTimelineDetailDto.Title,
        //        VideoUrl = courseTimelineDetailDto.VideoUrl,
        //        Topic = courseTimelineDetailDto.Topic,
        //        IsEnabled = courseTimelineDetailDto.IsEnabled
        //    };

        //    await _repository.UpdateAsync(courseTimelineDetail);

        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteAsync(Guid id)
        //{
        //    var deleted = await _repository.DeleteAsync(id);
        //    if (!deleted)
        //    {
        //        return NotFound($"No timeline detail found with ID {id}.");
        //    }

        //    return NoContent();
        //}


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

            // Chuyển đổi sang DTO
            var courseTimelineDetailsDto = courseTimelineDetailsList.Select(ct => new CourseTimelineDetailDto
            {
                Id = ct.Id,
                CourseTimelineId = ct.CourseTimelineId,
                Title = ct.Title,
                VideoUrl = ct.VideoUrl,
                Topic = ct.Topic,
                IsEnabled = ct.IsEnabled
            }).ToList();

            // Trả về danh sách chi tiết nếu tìm thấy
            return Ok(courseTimelineDetailsDto);
        }
    }
}
