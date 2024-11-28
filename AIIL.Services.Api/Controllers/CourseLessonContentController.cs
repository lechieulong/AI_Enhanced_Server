using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity.CourseFolder;
using IRepository;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseLessonContentController : ControllerBase
    {
        private readonly ICourseLessonContentRepository _courseLessonContentRepository;

        public CourseLessonContentController(ICourseLessonContentRepository courseLessonContentRepository)
        {
            _courseLessonContentRepository = courseLessonContentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseLessonContent>>> GetAll()
        {
            var contents = await _courseLessonContentRepository.GetAllAsync();
            return Ok(contents);
        }

        // GET: api/CourseLessonContent/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseLessonContent>> GetById(Guid id)
        {
            var content = await _courseLessonContentRepository.GetByIdAsync(id);
            if (content == null)
                return NotFound();

            return Ok(content);
        }

        // GET: api/CourseLessonContent/lesson/{courseLessonId}
        [HttpGet("lesson/{courseLessonId}")]
        public async Task<ActionResult<IEnumerable<CourseLessonContent>>> GetByCourseLessonId(Guid courseLessonId)
        {
            var contents = await _courseLessonContentRepository.GetByCourseLessonIdAsync(courseLessonId);
            return Ok(contents);
        }

        [HttpPost]
        public async Task<ActionResult<CourseLessonContent>> Create([FromBody] CourseLessonContentDto courseLessonContentDto)
        {
            if (courseLessonContentDto == null || courseLessonContentDto.CourseLessonId == Guid.Empty ||
                string.IsNullOrWhiteSpace(courseLessonContentDto.ContentType))
            {
                return BadRequest("Invalid course lesson content data.");
            }

            var courseLessonContent = new CourseLessonContent
            {
                Id = Guid.NewGuid(),
                CourseLessonId = courseLessonContentDto.CourseLessonId,
                ContentType = courseLessonContentDto.ContentType,
                ContentText = courseLessonContentDto.ContentText,
                ContentUrl = courseLessonContentDto.ContentUrl,
                Order = courseLessonContentDto.Order
            };

            var createdCourseLessonContent = await _courseLessonContentRepository.AddAsync(courseLessonContent);
            return CreatedAtAction(nameof(GetById), new { id = createdCourseLessonContent.Id }, createdCourseLessonContent);
        }


        // PUT: api/CourseLessonContent/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, CourseLessonContent content)
        {
            if (id != content.Id)
                return BadRequest("ID mismatch.");

            await _courseLessonContentRepository.UpdateAsync(content);
            return NoContent();
        }

        // DELETE: api/CourseLessonContent/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _courseLessonContentRepository.DeleteAsync(id);
            return NoContent();
        }

    }
}
