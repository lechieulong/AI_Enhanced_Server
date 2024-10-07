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
        public async Task<IActionResult> CreateTest([FromBody] TestModel model)
        {
            var result = await _testExamService.CreateTestAsync(model);
            return Ok(result);
        }
    }
}
