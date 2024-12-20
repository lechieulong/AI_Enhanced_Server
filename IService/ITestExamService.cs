

using Model.Test;
using Entity.Test;
using Microsoft.AspNetCore.Mvc;

namespace IService
{
    public interface ITestExamService
    {
        Task<TestModel> CreateTestAsync(Guid userId,TestModel model, int userRoleClaim);
        Task<TestResultWithExamDto> CalculateScore( Guid testId, Guid userId, SubmitTestDto model);
        Task<Dictionary<string, object>> GetExplainByTestId(TestExplainRequestDto model);
        Task<IEnumerable<TestExam>> GetPagedAdminTests( int pageNumber, int pageSize);
        //Task CreateSkillsAsync(Guid userId,Guid testId, Dictionary<string, SkillDto> model);
        Task<(IEnumerable<TestResultWithExamDto>, int)> GetTestResultByUserIdAsync(Guid courseId, string userId);
    }
}
