using Model;
using Model.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface ITestExamRepository
    {
        Task<TestResponseModel> AddTestAsync(TestRequestModel model);
        Task<TestResponseModel> GetTestByIdAsync(Guid id);
        Task UpdateTestAsync(Guid id, TestRequestModel model);
        Task DeleteTestAsync(Guid id);
    }
}
