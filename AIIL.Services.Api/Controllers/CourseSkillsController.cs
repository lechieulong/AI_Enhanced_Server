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
    public class CourseSkillsController : ControllerBase
    {
        private readonly ICourseSkillRepository _courseSkillRepository;

        public CourseSkillsController(ICourseSkillRepository courseSkillRepository)
        {
            _courseSkillRepository = courseSkillRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseSkill>>> GetAll()
        {
            var courseSkills = await _courseSkillRepository.GetAllAsync();
            return Ok(courseSkills);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseSkill>> GetById(Guid id)
        {
            var courseSkill = await _courseSkillRepository.GetByIdAsync(id);
            if (courseSkill == null)
                return NotFound();

            return Ok(courseSkill);
        }

        [HttpPost]
        public async Task<ActionResult<CourseSkill>> Create([FromBody] CourseSkillDto courseSkillDto)
        {
            // Kiểm tra dữ liệu đầu vào
            if (courseSkillDto == null || courseSkillDto.CourseId == Guid.Empty || string.IsNullOrWhiteSpace(courseSkillDto.Type))
            {
                return BadRequest("Invalid course skill data.");
            }

            // Chuyển đổi từ CourseSkillDto sang CourseSkill entity
            var courseSkill = new CourseSkill
            {
                Id = Guid.NewGuid(), // Tạo ID mới cho CourseSkill
                CourseId = courseSkillDto.CourseId,
                Type = courseSkillDto.Type,
                Description = courseSkillDto.Description
            };

            // Lưu CourseSkill vào cơ sở dữ liệu
            var createdCourseSkill = await _courseSkillRepository.AddAsync(courseSkill);
            return CreatedAtAction(nameof(GetById), new { id = createdCourseSkill.Id }, createdCourseSkill);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CourseSkill courseSkill)
        {
            if (id != courseSkill.Id)
                return BadRequest();

            var existingCourseSkill = await _courseSkillRepository.GetByIdAsync(id);
            if (existingCourseSkill == null)
                return NotFound();

            await _courseSkillRepository.UpdateAsync(courseSkill);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _courseSkillRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
        [HttpGet("Course/{courseId}/FullSkills")]
        public async Task<ActionResult<IEnumerable<string>>> GetSkillDescriptionsByCourseId(Guid courseId)
        {
            var courseSkills = await _courseSkillRepository.GetByCourseIdAsync(courseId);

            if (courseSkills == null || !courseSkills.Any())
                return NotFound("No skills found for this course.");

            var skillDescriptions = courseSkills.Select(skill => skill.Description).ToList();
            return Ok(skillDescriptions);
        }

        [HttpGet("Course/{courseId}")]
        public async Task<ActionResult<IEnumerable<CourseSkill>>> GetFullSkillsByCourseId(Guid courseId)
        {
            var courseSkills = await _courseSkillRepository.GetByCourseIdAsync(courseId);

            if (courseSkills == null || !courseSkills.Any())
                return NotFound("No skills found for this course.");

            return Ok(courseSkills);
        }


        [HttpGet("DescriptionByCourseLesson/{courseLessonId}")]
        public async Task<ActionResult<string>> GetDescriptionByCourseLessonId(Guid courseLessonId)
        {
            var description = await _courseSkillRepository.GetDescriptionByCourseLessonIdAsync(courseLessonId);
            if (string.IsNullOrEmpty(description))
                return NotFound("No description found for this CourseLessonId.");

            return Ok(description);
        }

        [HttpGet("DescriptionByCoursePart/{coursePartId}")]
        public async Task<ActionResult<string>> GetDescriptionByCoursePartId(Guid coursePartId)
        {
            var description = await _courseSkillRepository.GetDescriptionByCoursePartIdAsync(coursePartId);
            if (string.IsNullOrEmpty(description))
                return NotFound("No description found for this CoursePartId.");

            return Ok(description);
        }
        [HttpGet("DescriptionBySkill/{skillId:guid}")]
        public async Task<IActionResult> GetDescriptionBySkillId(Guid skillId)
        {
            var courseSkill = await _courseSkillRepository.GetBySkillIdAsync(skillId);

            if (courseSkill == null)
            {
                return NotFound("No description found for the provided Skill ID.");
            }

            return Ok(new { Description = courseSkill.Description });
        }
    }
}
