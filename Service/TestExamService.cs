using IRepository;
using IService;
using Model.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class TestExamService : ITestExamService
    {
        private readonly ITestExamRepository _testExamRepository;

        public TestExamService(ITestExamRepository testExamRepository)
        {
            _testExamRepository = testExamRepository;
        }

        public async Task<TestResponseModel> CreateTestAsync(TestRequestModel model)
        {
            return await _testExamRepository.AddTestAsync(model);
        }

        public async Task<TestResponseModel> GetTestByIdAsync(Guid id)
        {
            return await _testExamRepository.GetTestByIdAsync(id);
        }

        public async Task UpdateTestAsync(Guid id, TestRequestModel model)
        {
            await _testExamRepository.UpdateTestAsync(id, model);
        }

        public async Task DeleteTestAsync(Guid id)
        {
            await _testExamRepository.DeleteTestAsync(id);
        }
    }
}
