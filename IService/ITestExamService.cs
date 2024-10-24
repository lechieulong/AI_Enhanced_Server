

using Model.Test;

namespace IService
{
    public interface ITestExamService
    {
        Task<TestModel> CreateTestAsync(TestModel model);
        Task CreateSkillsAsync(Guid testId, Dictionary<string, SkillDto> model);
    }
}
