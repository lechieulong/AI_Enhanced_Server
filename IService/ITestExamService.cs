

using Model.Test;
using Entity.Test;
using Microsoft.AspNetCore.Mvc;

namespace IService
{
    public interface ITestExamService
    {
        Task<TestModel> CreateTestAsync(Guid userId,TestModel model, string userRoleClaim);
        Task<TestResult> CalculateScore( Guid testId, Guid userIds, Dictionary<string, UserAnswersDto> model);

        //Task CreateSkillsAsync(Guid userId,Guid testId, Dictionary<string, SkillDto> model);
    }
}
