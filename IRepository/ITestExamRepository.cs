using Entity.Test;
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
        Task<TestModel> AddTestAsync(Guid userId, TestModel model);
        Task<IEnumerable<TestExam>> GetAllTestsAsync(Guid userId);
        Task<TestExam> GetTestAsync(Guid id);
        Task<List<Skill>> GetSkills(Guid testId);
        Task<List<Part>> GetParts(Guid skillId);
        Task ImportQuestionAsync(List<Question> questions, Guid userId);
        Task<List<Question>> GetAllQuestionsAsync(Guid userId);
        Task AddQuestionsAsync(List<Question> questionModels);
        Task<Question> GetQuestionByIdAsync(Guid id); // Updated to match implementation
        Task UpdateQuestionAsync(QuestionResponse updatedQuestion); // Added
        Task DeleteQuestionAsync(Guid id); // Added
        Task CreateSkillsAsync(Guid userId, Guid testId, Dictionary<string, SkillDto> model);
    }
}
