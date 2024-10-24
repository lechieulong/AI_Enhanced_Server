using Common;
using Entity;
using Entity.Data;
using Entity.Test;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Model.Test;
using System.Reflection.Metadata.Ecma335;

namespace Repository
{
    public class TestExamRepository : ITestExamRepository
    {
        private readonly AppDbContext _context;

        public TestExamRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddSkillAsync(Skill skill)
        {
            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
        }

        public async Task<TestModel> AddTestAsync(TestModel model)
        {
            var newTest = new TestExam
            {
                Id = Guid.NewGuid(),
                TestName = model.TestName,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                CreateBy = model.CreateBy,
            };
            _context.TestExams.Add(newTest);

            foreach (var classId in model.ClassIds)
            {
                var classRelation = new ClassRelationShip
                {
                    Id = Guid.NewGuid(), 
                    TestId = newTest.Id, 
                    ClassId = classId   
                };

                _context.ClassRelationShip.Add(classRelation); 
            }

            await _context.SaveChangesAsync();

            model.Id = newTest.Id;
            return model;
        }

        public async Task<List<Question>> GetAllQuestionsAsync(Guid userId)
        {
            return await _context.Questions
                                 .Where(q => q.UserId == userId)
                                 .Include(q => q.Answers) 
                                 .ToListAsync();
        }

        public async Task AddQuestionsAsync(List<Question> questions)
        {
            foreach (var question in questions)
            {
                var newQuestion = new Question
                {
                    Id = Guid.NewGuid(),
                    QuestionName = question.QuestionName,
                    QuestionType = question.QuestionType,
                    Skill = question.Skill,
                    PartNumber = question.PartNumber,
                    UserId = question.UserId, // Ensure UserId is set here
                    Answers = new List<Answer>() // Initialize the Answers collection
                };

                // Check if there are any answers to add
                if (question.Answers != null)
                {
                    foreach (var answer in question.Answers)
                    {
                        // Create a new Answer entity
                        var newAnswer = new Answer
                        {
                            Id = Guid.NewGuid(), // Assign a new Guid
                            QuestionId = newQuestion.Id, // Associate with the new question
                            AnswerText = answer.AnswerText,
                            IsCorrect = answer.IsCorrect
                        };

                        _context.Answers.Add(newAnswer); // Add to the Answers table
                        newQuestion.Answers.Add(newAnswer); // Add to the question's Answers collection
                    }
                }

                _context.Questions.Add(newQuestion); // Add the question to the context
            }

            await _context.SaveChangesAsync(); // Save all changes in one go
        }

        public async Task ImportQuestionAsync(List<Question> questions, Guid userId)
        {
            foreach (var question in questions)
            {
                // Create a new Question entity
                var newQuestion = new Question
                {
                    Id = Guid.NewGuid(),
                    QuestionName = question.QuestionName,
                    QuestionType = question.QuestionType,
                    Skill = question.Skill,
                    PartNumber = question.PartNumber,
                    UserId = userId,
                    Answers = new List<Answer>() // Initialize the Answers collection
                };

                // Check if there are any answers to add
                if (question.Answers != null)
                {
                    foreach (var answer in question.Answers)
                    {
                        // Create a new Answer entity
                        var newAnswer = new Answer
                        {
                            Id = Guid.NewGuid(), // Assign a new Guid
                            QuestionId = newQuestion.Id, // Associate with the new question
                            AnswerText = answer.AnswerText,
                            IsCorrect = answer.IsCorrect
                        };

                        _context.Answers.Add(newAnswer); // Add to the Answers table
                                                         // Optionally, you can also add to the question's Answers collection
                        newQuestion.Answers.Add(newAnswer);
                    }
                }

                _context.Questions.Add(newQuestion); // Assuming you have a DbSet<Question> Questions in your context
            }

            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<TestExam>> GetAllTestsAsync(Guid userId)
        {
            return await _context.TestExams
                .Where(test => test.CreateBy == userId) 
                .ToListAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Answers) // Include answers for the question
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task UpdateQuestionAsync(QuestionResponse updatedQuestion)
        {
            // Retrieve the existing question from the database, including its answers
            var existingQuestion = await _context.Questions
                .Include(q => q.Answers) // Include the answers related to the question
                .FirstOrDefaultAsync(q => q.Id == updatedQuestion.Id);

            if (existingQuestion != null)
            {
                // Update the question properties
                existingQuestion.QuestionName = updatedQuestion.QuestionName;
                existingQuestion.QuestionType = (int)updatedQuestion.QuestionType;
                existingQuestion.Skill = updatedQuestion.Skill;
                existingQuestion.PartNumber = updatedQuestion.PartNumber;

                // Update the answers
                // Clear existing answers only if they need to be removed
                var existingAnswers = existingQuestion.Answers.ToList(); // Get a copy of existing answers

                foreach (var answer in updatedQuestion.Answers)
                {
                    var existingAnswer = existingAnswers.FirstOrDefault(a => a.Id == answer.Id);
                    if (existingAnswer != null)
                    {
                        // Update existing answer
                        existingAnswer.AnswerText = answer.AnswerText;
                        existingAnswer.IsCorrect = answer.IsCorrect;
                        existingAnswers.Remove(existingAnswer); // Remove from list to keep track of updates
                    }
                    else
                    {
                        // Add new answer
                        existingQuestion.Answers.Add(new Answer
                        {
                            Id = Guid.NewGuid(), // Ensure a new ID is assigned if it's a new answer
                            AnswerText = answer.AnswerText,
                            IsCorrect = answer.IsCorrect
                        });
                    }
                }

                // Remove answers that were not included in the updated question
                foreach (var answerToRemove in existingAnswers)
                {
                    _context.Answers.Remove(answerToRemove);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException("Question not found");
            }
        }
        public async Task DeleteQuestionAsync(Guid id)
        {
            // Find the question to delete
            var questionToDelete = await _context.Questions.FindAsync(id);
            if (questionToDelete != null)
            {
                // Remove the question from the context
                _context.Questions.Remove(questionToDelete);
                await _context.SaveChangesAsync(); // Save changes to the database
            }
        }



    }
}
