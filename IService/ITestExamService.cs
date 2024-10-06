

using Model.Test;

namespace IService
{
    public interface ITestExamService
    {
        Task<TestModel> CreateTestAsync(TestModel model);
    }
}
