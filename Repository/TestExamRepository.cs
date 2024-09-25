using Entity;
using Entity.Data;
using Entity.Test;
using IRepository;
using Model.Test;

namespace Repository
{
    public class TestExamRepository : ITestExamRepository
    {
        private readonly AppDbContext _context;

        public TestExamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TestResponseModel> AddTestAsync(TestRequestModel model)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Add the TestExam entity
                    var testEntity = new TestExam
                    {
                        Id = new Guid(),
                        TestName = model.TestName,
                        Duration = 60,
                        StartTime = model.StartTime,
                        EndTime = model.EndTime,
                        CreateAt = DateTime.UtcNow,
                        UpdateAt = DateTime.UtcNow
                    };

                    _context.TestExam.Add(testEntity);
                    await _context.SaveChangesAsync();

                    // Add related SkillTestExam entities
                    foreach (var skillTestModel in model.Skills)
                    {
                        await AddSkillTestAsync(testEntity.Id, skillTestModel);
                    }

                    await transaction.CommitAsync();

                    return new TestResponseModel
                    {
                        Id = testEntity.Id,
                        TestName = testEntity.TestName,
                        Duration = testEntity.Duration,
                        StartTime = testEntity.StartTime,
                        EndTime = testEntity.EndTime,
                        // You can map other properties as needed
                    };
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private async Task AddSkillTestAsync(Guid testId, SkillRequest skillTestModel)
        {
            // Add SkillTestExam entity
            var skillTest = new SkillTestExam
            {
                TestId = testId,
                SkillType = (int)skillTestModel.SKillType,
                Duration = skillTestModel.Duration
            };

            _context.SkillTestExam.Add(skillTest);
            await _context.SaveChangesAsync();

            // Add PartSkill entities
            foreach (var partSkillModel in skillTestModel.PartSkills)
            {
                await AddPartSkillAsync(skillTest.Id, partSkillModel);
            }
        }

        private async Task AddPartSkillAsync(Guid skillTestId, PartRequestModel partSkillModel)
        {
            var partSkill = new PartSkill
            {
                SkillId = skillTestId,
                PartNumber = partSkillModel.PartNumber,
                ContentText = partSkillModel.ContentText
            };

            _context.PartSkill.Add(partSkill);
            await _context.SaveChangesAsync();

            // Add QuestionTypePart entities
            foreach (var questionTypePartModel in partSkillModel.QuestionTypeParts)
            {
                await AddQuestionTypePartAsync(partSkill.Id, questionTypePartModel);
            }
        }

        private async Task AddQuestionTypePartAsync(Guid partSkillId, QuestionTypePartRequestModel questionTypePartModel)
        {
            var questionTypePart = new QuestionTypePart
            {
                PartId = partSkillId,
                QuestionGuide = questionTypePartModel.QuestionGuide,
                QuestionType = (int)questionTypePartModel.QuestionType
            };

            _context.QuestionTypePart.Add(questionTypePart);
            await _context.SaveChangesAsync();

            foreach (var questionModel in questionTypePartModel.Questions)
            {
                await AddQuestionAsync(questionTypePart.Id, questionModel);
            }
        }

        private async Task AddQuestionAsync(Guid questionTypePartId, QuestionRequestModel questionModel)
        {
            // Add Question entity
            var question = new Question
            {
                TypePartId = questionTypePartId,
                QuestionName = questionModel.QuestionName,
                MaxMarks = questionModel.MaxMarks
            };

            _context.Question.Add(question);
            await _context.SaveChangesAsync();

            // Add related Answers
            foreach (var answerModel in questionModel.Answers)
            {
                await AddAnswerAsync(question.Id, answerModel);
            }
        }

        private async Task AddAnswerAsync(Guid questionId, AnswerRequest answerModel)
        {
            // Add Answer entity
            var answer = new Answer
            {
                QuestionId = questionId,
                AnswerFilling = answerModel.AnswerFilling,
                AnswerTrueFalse = answerModel.AnswerTrueFalse,
            };

            _context.Answer.Add(answer);
            await _context.SaveChangesAsync();

            // Add AnswerOptions
            foreach (var optionModel in answerModel.AnswerOptionsRequest)
            {
                var answerOption = new AnswerOptions
                {
                    Id  = new Guid(),
                    AnswerId = answer.Id,
                    AnswerText = optionModel.AnswerText,
                    IsCorrect = optionModel.IsCorrect,
                };

                _context.AnswerOptions.Add(answerOption);
            }

            // Add AnswerMatching
            foreach (var matchingModel in answerModel.AnswerMatchingRequest)
            {
                var answerMatching = new AnswerMatching
                {
                    AnswerId = answer.Id,
                    Heading = matchingModel.Heading,
                    Matching = matchingModel.Matching
                };

                _context.AnswerMatching.Add(answerMatching);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<TestResponseModel> GetTestByIdAsync(Guid id)
        {
            var testEntity = await _context.TestExam.FindAsync(id);
            if (testEntity == null) return null;

            return new TestResponseModel
            {
                Id = testEntity.Id,
                TestName = testEntity.TestName,
                Duration = testEntity.Duration,
                StartTime = testEntity.StartTime,
                EndTime = testEntity.EndTime,
                // Map other properties as needed
            };
        }

        public async Task UpdateTestAsync(Guid id, TestRequestModel model)
        {
            var testEntity = await _context.TestExam.FindAsync(id);
            if (testEntity == null) return;

            // Update properties
            testEntity.TestName = model.TestName;
            testEntity.Duration = 60;
            testEntity.StartTime = model.StartTime;
            testEntity.EndTime = model.EndTime;
            testEntity.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTestAsync(Guid id)
        {
            var testEntity = await _context.TestExam.FindAsync(id);
            if (testEntity == null) return;

            _context.TestExam.Remove(testEntity);
            await _context.SaveChangesAsync();
        }
    }
}
