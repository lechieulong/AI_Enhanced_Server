using System;
using System.Threading.Tasks;
using Entity;
using Entity.CourseFolder;
using IRepository;
using Microsoft.AspNetCore.Mvc;
using Model;
using Repository;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassFileController : ControllerBase
    {
        private readonly IClassFileRepository _classFileRepository;

        public ClassFileController(IClassFileRepository classFileRepository)
        {
            _classFileRepository = classFileRepository;
        }

        [HttpGet("class/{classId}")]
        public async Task<IActionResult> GetAllByClassId(Guid classId)
        {
            var classFiles = await _classFileRepository.GetAllByClassIdAsync(classId);
            if (classFiles == null || !classFiles.Any())
                return NotFound("No class files found for the specified class ID.");

            return Ok(classFiles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var classFile = await _classFileRepository.GetByIdAsync(id);
            if (classFile == null)
                return NotFound($"Class file with ID {id} not found.");

            return Ok(classFile);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClassFileDto classFileDto)
        {
            if (classFileDto == null || string.IsNullOrWhiteSpace(classFileDto.FilePath) ||
                classFileDto.ClassId == Guid.Empty || string.IsNullOrWhiteSpace(classFileDto.Topic) ||
                string.IsNullOrWhiteSpace(classFileDto.Description))
            {
                return BadRequest("Invalid class file data.");
            }

            var classFile = new ClassFile
            {
                Id = Guid.NewGuid(),
                FilePath = classFileDto.FilePath,
                ClassId = classFileDto.ClassId,
                UploadDate = classFileDto.UploadDate,
                Topic = classFileDto.Topic,
                Description = classFileDto.Description
            };

            await _classFileRepository.CreateAsync(classFile);

            return CreatedAtAction(nameof(GetById), new { id = classFile.Id }, classFile);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ClassFileDto updatedClassFileDto)
        {


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedClassFile = await _classFileRepository.UpdateAsync(id, updatedClassFileDto);

            if (updatedClassFile == null)
                return NotFound($"Class file with ID {id} not found.");

            return Ok(updatedClassFile);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleted = await _classFileRepository.DeleteAsync(id);

            if (!isDeleted)
                return NotFound($"Class file with ID {id} not found.");

            return NoContent();
        }
    }
}
