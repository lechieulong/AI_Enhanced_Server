using Entity.Test;
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
        Task<TestModel> AddTestAsync(TestModel model);
        Task<IEnumerable<TestExam>> GetAllTestsAsync(Guid userId);
    }
}
