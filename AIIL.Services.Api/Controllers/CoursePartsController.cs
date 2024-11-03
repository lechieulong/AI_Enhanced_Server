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
        public async Task<ActionResult<CoursePart>> Create(CoursePart coursePart)
        {
            if (coursePart == null)
                return BadRequest();

            var createdCoursePart = await _coursePartRepository.AddAsync(coursePart);
            return CreatedAtAction(nameof(GetById), new { id = createdCoursePart.Id }, createdCoursePart);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CoursePart coursePart)
        {
            if (id != coursePart.Id)
                return BadRequest();

            var existingCoursePart = await _coursePartRepository.GetByIdAsync(id);
            if (existingCoursePart == null)
                return NotFound();

            await _coursePartRepository.UpdateAsync(coursePart);
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
    }
}
