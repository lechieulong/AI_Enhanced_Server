using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;
using Repository;
using Model;
using IRepository;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursePartsController : ControllerBase
    {
        private readonly ICoursePartRepository _coursePartRepository;

        public CoursePartsController(ICoursePartRepository coursePartRepository)
        {
            _coursePartRepository = coursePartRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CoursePart>>> GetAll()
        {
            var courseParts = await _coursePartRepository.GetAllAsync();
            return Ok(courseParts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CoursePart>> GetById(Guid id)
        {
            var coursePart = await _coursePartRepository.GetByIdAsync(id);
            if (coursePart == null)
                return NotFound();

            return Ok(coursePart);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CoursePartDto coursePartDto)
        {
            // Kiểm tra dữ liệu đầu vào
            if (coursePartDto == null || coursePartDto.CourseSkillId == Guid.Empty ||
                string.IsNullOrWhiteSpace(coursePartDto.Title))
            {
                return BadRequest("Invalid course part data.");
            }

            // Lấy giá trị Order cao nhất của CoursePart
            var maxOrder = await _coursePartRepository.GetMaxOrderByCourseSkillIdAsync(coursePartDto.CourseSkillId);

            // Tạo đối tượng CoursePart mới
            var coursePart = new CoursePart
            {
                Id = Guid.NewGuid(), // Tạo ID mới
                CourseSkillId = coursePartDto.CourseSkillId,
                Title = coursePartDto.Title,
                Order = maxOrder + 1 // Tăng giá trị Order
            };

            // Lưu đối tượng vào cơ sở dữ liệu
            await _coursePartRepository.AddAsync(coursePart);

            return CreatedAtAction(nameof(GetById), new { id = coursePart.Id }, coursePart);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CoursePartDto coursePartDto)
        {
            // Kiểm tra dữ liệu đầu vào
            if (coursePartDto == null || id == Guid.Empty ||
                string.IsNullOrWhiteSpace(coursePartDto.Title))
            {
                return BadRequest("Invalid course part data.");
            }

            var existingCoursePart = await _coursePartRepository.GetByIdAsync(id);
            if (existingCoursePart == null)
                return NotFound();

            // Cập nhật các thuộc tính từ DTO
            existingCoursePart.CourseSkillId = coursePartDto.CourseSkillId;
            existingCoursePart.Title = coursePartDto.Title;
            existingCoursePart.Order = coursePartDto.Order;
            await _coursePartRepository.UpdateAsync(existingCoursePart);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _coursePartRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        [HttpGet("ByCourse/{courseId}")]
        public async Task<ActionResult<IEnumerable<CoursePart>>> GetByCourseId(Guid courseId)
        {
            var courseParts = await _coursePartRepository.GetByCourseIdAsync(courseId);
            if (courseParts == null || !courseParts.Any())
                return NotFound();

            return Ok(courseParts);
        }
        [HttpGet("ByCourseSkill/{courseSkillId}")]
        public async Task<ActionResult<IEnumerable<CoursePart>>> GetByCourseSkillId(Guid courseSkillId)
        {
            var courseParts = await _coursePartRepository.GetByCourseSkillIdAsync(courseSkillId);
            if (courseParts == null || !courseParts.Any())
                return NotFound();

            return Ok(courseParts);
        }

    }
}
