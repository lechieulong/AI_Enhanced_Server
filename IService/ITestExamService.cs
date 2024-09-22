

using Model.Test;

namespace IService
{
    public interface ITestExamService
    {
        Task<TestResponseModel> CreateTestAsync(TestRequestModel model);
        Task<TestResponseModel> GetTestByIdAsync(Guid id);
        Task UpdateTestAsync(Guid id, TestRequestModel model);
        Task DeleteTestAsync(Guid id);
    }
}
