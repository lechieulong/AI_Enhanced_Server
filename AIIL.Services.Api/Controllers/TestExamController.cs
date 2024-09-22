using IRepository;
using IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Test;
using System;
using System.Threading.Tasks;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestExamController : ControllerBase
    {
        private readonly ITestExamService _testExamService;

        public TestExamController(ITestExamService testExamService)
        {
            _testExamService = testExamService;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateTest([FromBody] TestRequestModel model)
        {
            var result = await _testExamService.CreateTestAsync(model);
            return CreatedAtAction(nameof(GetTest), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTest(Guid id)
        {
            var result = await _testExamService.GetTestByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTest(Guid id, [FromBody] TestRequestModel model)
        {
            var existingTest = await _testExamService.GetTestByIdAsync(id);
            if (existingTest == null)
                return NotFound();

            await _testExamService.UpdateTestAsync(id, model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTest(Guid id)
        {
            var existingTest = await _testExamService.GetTestByIdAsync(id);
            if (existingTest == null)
                return NotFound();

            await _testExamService.DeleteTestAsync(id);
            return NoContent();
        }
    }
}
