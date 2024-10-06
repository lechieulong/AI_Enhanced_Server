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

        public async Task<TestModel> CreateTestAsync(TestModel model)
        {
            return await _testExamRepository.AddTestAsync(model);
        }
      
    }
}
