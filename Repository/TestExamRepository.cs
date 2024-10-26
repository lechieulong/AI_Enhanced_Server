using AutoMapper;
using Common;
using Entity;
using Entity.Data;
using Entity.Test;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Model.Test;

namespace Repository
{
    public class TestExamRepository : ITestExamRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TestExamRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task CreateSkillsAsync(Guid userId, Guid testId, Dictionary<string, SkillDto> model)
        {
            // Check if the model is null or empty
            if (model == null || !model.Any())
                throw new ArgumentException("Model cannot be null or empty.", nameof(model));

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var skillKeyValue in model)
                {
                    var skillDto = skillKeyValue.Value;

                    // Skip if the skillDto is null
                    if (skillDto == null)
                        continue;

                    var skill = new Skill
                    {
                        Id = Guid.NewGuid(),
                        TestId = testId,
                        Duration = skillDto.Duration,
                        Type = (int)skillDto.Type,
                        Parts = new List<Part>()
                    };

                    // Check if Parts are not null or empty
                    if (skillDto.Parts != null && skillDto.Parts.Any())
                    {
                        int partIndex = 1;

                        foreach (var partDto in skillDto.Parts)
                        {
                            // Skip if the partDto is null
                            if (partDto == null)
                                continue;

                            var part = new Part
                            {
                                Id = Guid.NewGuid(),
                                PartNumber = partIndex,
                                ContentText = partDto.ContentText,
                                Audio = partDto.Audio,
                                Image = partDto.Image,
                                Skill = skill,
                                Sections = new List<Section>()
                            };

                            // Check if Sections are not null or empty
                            if (partDto.Sections != null && partDto.Sections.Any())
                            {
                                foreach (var sectionDto in partDto.Sections)
                                {
                                    // Skip if the sectionDto is null
                                    if (sectionDto == null)
                                        continue;

                                    var section = new Section
                                    {
                                        Id = Guid.NewGuid(),
                                        SectionGuide = sectionDto.SectionGuide,
                                        SectionType = sectionDto.SectionType,
                                        Part = part,
                                        SectionQuestions = new List<SectionQuestion>()
                                    };

                                    // Check if Questions are not null or empty
                                    if (sectionDto.Questions != null && sectionDto.Questions.Any())
                                    {
                                        foreach (var questionDto in sectionDto.Questions)
                                        {
                                            // Skip if the questionDto is null
                                            if (questionDto == null)
                                                continue;

                                            var question = new Question
                                            {
                                                Id = Guid.NewGuid(),
                                                UserId = userId,
                                                QuestionName = questionDto.QuestionName,
                                                QuestionType = questionDto.QuestionType,
                                                Answers = new List<Answer>() // Initialize Answers collection
                                            };

                                            // Check if Answers are not null or empty
                                            if (questionDto.Answers != null && questionDto.Answers.Any())
                                            {
                                                foreach (var answerDto in questionDto.Answers)
                                                {
                                                    // Skip if the answerDto is null
                                                    if (answerDto == null)
                                                        continue;

                                                    var newAnswer = new Answer
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        AnswerText = answerDto.AnswerText,
                                                        TypeCorrect = (int)answerDto.IsCorrect
                                                    };

                                                    question.Answers.Add(newAnswer); // Add to the question's Answers collection
                                                }
                                            }

                                            var sectionQuestion = new SectionQuestion
                                            {
                                                Id = Guid.NewGuid(),
                                                Section = section,
                                                Question = question
                                            };

                                            section.SectionQuestions.Add(sectionQuestion); // Add the question to the section
                                        }
                                    }

                                    part.Sections.Add(section); // Add the section to the part
                                }
                            }

                            partIndex++;
                            skill.Parts.Add(part); 
                        }
                    }

                    await _context.Skills.AddAsync(skill);
                }

                await _context.SaveChangesAsync(); // Save all changes
                await transaction.CommitAsync(); // Commit the transaction
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(); // Rollback on error
                throw; // Rethrow the exception
            }
        }

        public async Task<TestModel> AddTestAsync(Guid userId, TestModel model)
        {
            var newTest = new TestExam
            {
                Id = Guid.NewGuid(),
                TestName = model.TestName,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                UserID = userId,
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
                            TypeCorrect = answer.TypeCorrect
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
                            TypeCorrect = answer.TypeCorrect
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
                .Where(test => test.UserID == userId) 
                .ToListAsync();
        }

        public async Task<List<Skill>> GetSkills(Guid testId)
        {
            return await _context.Skills.Where(skill => skill.TestId == testId).ToListAsync();
        }
        public async Task<List<Part>> GetParts(Guid skillId)
        {
            return await _context.Parts.Where(part => part.Skill.Id == skillId).ToListAsync();
        }

        public async Task<TestExam> GetTestAsync(Guid id)
        {
            return await _context.TestExams
                .FirstOrDefaultAsync(test => test.Id == id);
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
                        existingAnswer.TypeCorrect = answer.IsCorrect;
                        existingAnswers.Remove(existingAnswer); // Remove from list to keep track of updates
                    }
                    else
                    {
                        // Add new answer
                        existingQuestion.Answers.Add(new Answer
                        {
                            Id = Guid.NewGuid(), // Ensure a new ID is assigned if it's a new answer
                            AnswerText = answer.AnswerText,
                            TypeCorrect = answer.IsCorrect
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
