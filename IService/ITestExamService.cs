﻿

using Model.Test;

namespace IService
{
    public interface ITestExamService
    {
        Task<TestModel> CreateTestAsync(Guid userId,TestModel model, string userRoleClaim);
        //Task CreateSkillsAsync(Guid userId,Guid testId, Dictionary<string, SkillDto> model);
    }
}
