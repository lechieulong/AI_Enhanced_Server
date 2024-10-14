using AutoMapper;
using IRepository;
using IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ITestExamRepository _testRepository;
        private readonly IMapper _mapper;


        public TestExamController(ITestExamService testExamService, ITestExamRepository testRepository, IMapper mapper)
        {
            _testExamService = testExamService;
            _testRepository = testRepository;
            _mapper = mapper;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateTest([FromBody] TestModel model)
        {
            var result = await _testExamService.CreateTestAsync(model);
            return Ok(result);
        }

        [HttpGet("")]
        public async Task<IEnumerable<TestModel>> GetAllTestsAsync([FromRoute] Guid userId)
        {
            var tests = await _testRepository.GetAllTestsAsync(userId); 
            return _mapper.Map<IEnumerable<TestModel>>(tests);
        }
    }
}
