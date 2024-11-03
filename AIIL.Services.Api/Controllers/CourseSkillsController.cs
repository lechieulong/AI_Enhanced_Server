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
        public async Task<ActionResult<CourseSkill>> Create(CourseSkill courseSkill)
        {
            if (courseSkill == null)
                return BadRequest();

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
    }
}
