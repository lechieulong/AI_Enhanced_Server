using Entity;
using Entity.Data;
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
                    var testEntity = new TestExam
                    {
                        TestName = model.TestName,
                        Duration = model.Duration,
                        StartTime = model.StartTime,
                        EndTime = model.EndTime,
                    };

                    _context.TestExam.Add(testEntity);
                    await _context.SaveChangesAsync();

                    // Add related entities like PartSkill and Question
                    foreach (var part in model.Parts)
                    {
                        var partSkill = new PartSkill
                        {
                            Id = testEntity.Id,
                            PartNumber = part.PartNumber,
                            SkillTest = part.SkillTest,
                            ContentText = part.ContentText,
                            AudioURL = part.AudioUrl
                        };

                        _context.PartSkill.Add(partSkill);
                        await _context.SaveChangesAsync();

                        foreach (var typePart in part.QuestionTypeParts)
                        {
                            var questionTypePart = new QuestionTypePart
                            {
                                Id = partSkill.Id,
                                QuestionGuide = typePart.QuestionGuide,
                                QuestionType = typePart.QuestionType
                            };

                            _context.QuestionTypePart.Add(questionTypePart);
                            await _context.SaveChangesAsync();

                            foreach (var question in typePart.Questions)
                            {
                                var questionEntity = new Question
                                {
                                    Id = questionTypePart.Id,
                                    QuestionName = question.QuestionName,
                                    Answer = question.Answer,
                                    MaxMarks = question.MaxMarks
                                };

                                _context.Question.Add(questionEntity);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new TestResponseModel
                    {
                        Id = testEntity.Id,
                        TestName = testEntity.TestName,
                        Duration = testEntity.Duration,
                        StartTime = testEntity.StartTime,
                        EndTime = testEntity.EndTime,
                        // Mapping other properties
                    };
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
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
            };
        }

        public async Task UpdateTestAsync(Guid id, TestRequestModel model)
        {
            var testEntity = await _context.TestExam.FindAsync(id);
            if (testEntity == null) return;

            // Update properties
            testEntity.TestName = model.TestName;
            testEntity.Duration = model.Duration;
            testEntity.StartTime = model.StartTime;
            testEntity.EndTime = model.EndTime;
            // Update other properties

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
