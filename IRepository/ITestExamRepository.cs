using Entity.Test;
using Microsoft.EntityFrameworkCore;
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
        Task<string> GetQuestionNameById(Guid questionId);
        Task<string> GetContentText(Guid partId);
        Task<List<Answer>> GetCorrectAnswers(Guid questionId, int sectionType, int skill);
        Task<(IEnumerable<TestExam> tests, int totalCount)> GetTestsAsync(int page, int pageSize);
        Task<TestExam> UpdateTestAsync(TestExam testExam);
        Task<bool> DeleteTestAsync(Guid id);
        Task SaveUserAnswerAsync(List<UserAnswers> userAnswers);
        Task<int> GetAttemptCountByTestAndUserAsync(Guid testId, Guid userId);
        Task AddAttemptTestForYear(Guid userId, int year);
        Task UpdateExplainQuestionAsync(Guid questionId, string explain);
        Task<List<AttempTest>> GetAttemptTests(Guid userId);
        Task SaveTestResultAsync(TestResult testResult);
        Task<List<Answer>> GetAnswerByQuestionId(Guid questionId);
        Task<int> GetTotalQuestionBySkillId(Guid skillId);
        Task<TestModel> AddTestAsync(Guid userId, TestModel model, int role);
        Task<IEnumerable<TestExam>> GetAllTestsAsync(Guid userId);
        Task<TestExam> GetTestAsync(Guid id);
        Task<List<TestResult>> GetResultTest( Guid userId, List<Guid> ids);
        Task<TestExam> GetTestBySecionCourseId(Guid id);

        Task<List<Skill>> GetSkills(Guid testId);
        Task<Skill> GetSkillExplainByIdAsync(Guid SkillId, List<int> parts);
        Task<List<Skill>> GetSkillsExplainByTestIdAsync(Guid testId, List<int> parts);
        Task<Skill> GetSkillByIdAsync(Guid SkillId);
        Task<List<Skill>> GetSkillsByTestIdAsync(Guid testId);
        Task<List<UserAnswers>> GetUserAnswersByTestId(Guid testId,  Guid userId);
        Task<List<Part>> GetParts(Guid skillId);
        Task ImportQuestionAsync(List<Question> questions, Guid userId);
        Task<List<Question>> GetQuestionsBySecionTypeAsync(Guid userId, int sectionType, int page, int pageSize);
        Task<List<Question>> GetQuestionsAsync(Guid userId,  int page, int pageSize);
        Task<List<TestResult>> GetTestSubmittedAsync(Guid userId, int page, int pageSize);
        Task<object> GetTestAnalysisAttempt(Guid userId);
        Task AddQuestionsAsync(List<Question> questionModels);
        Task<Question> GetQuestionByIdAsync(Guid id); // Updated to match implementation
        Task UpdateQuestionAsync(QuestionResponse updatedQuestion); // Added
        Task DeleteQuestionAsync(Guid id); // Added
        Task CreateSkillsAsync(Guid userId, Guid testId, Dictionary<string, SkillDto> model);
        Task<List<TestExam>> GetTestExamByLessonIdAsync(Guid lessonId);//Get Test
        Task<List<TestExam>> GetTestExamsByClassIdAsync(Guid classId);

    }
}
