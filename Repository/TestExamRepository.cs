using AutoMapper;
using Common;
using Entity;
using Entity.CourseFolder;
using Entity.Data;
using Entity.Test;
using IRepository;
using IService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Model;
using Model.Test;
using Model.Utility;
using Newtonsoft.Json;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Repository
{
    public class TestExamRepository : ITestExamRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAzureService _azureService;
        private readonly IServiceProvider _serviceProvider;
        public TestExamRepository(AppDbContext context, IMapper mapper, IAzureService azureService, IServiceProvider serviceProvider)
        {
            _context = context;
            _mapper = mapper;
            _azureService = azureService;
            _serviceProvider = serviceProvider;
        }
        public async Task<List<TestExam>> getAll()
        {
           return await _context.TestExams.ToListAsync();
        }
        public async Task SaveUserAnswerAsync(List<UserAnswers> userAnswers)
        {
            await _context.UserAnswers.AddRangeAsync(userAnswers);
            await _context.SaveChangesAsync();
        }

        public async Task<Part> GetPartNumber(Guid partId) {
            return await _context.Parts.Where(p => p.Id == partId).FirstOrDefaultAsync();
        }
        public async Task<string> GetQuestionNameById(Guid questionId)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.Id == questionId);

            return question?.QuestionName; 
        }

        public async Task<string> GetContentText(Guid partId)
        {
            return await _context.Parts
                .Where(p => p.Id == partId)
                .Select(p => p.ContentText) // Select only the `Context` property
                .FirstOrDefaultAsync(); // Get the first or default value
        }

        public async Task<string> GetUrlAudioByPartId(Guid partId)
        {
            return await _context.Parts
                .Where(p => p.Id == partId)
                .Select(p => p.Audio) // Select only the `Context` property
                .FirstOrDefaultAsync(); // Get the first or default value
        }
        
        public async Task<List<Answer>> GetCorrectAnswers(Guid questionId, int sectionType, int skill)
        {
            var result = new List<Answer>();

            if (skill == 0 )
            {
                if (sectionType == 1 || sectionType == 2 || sectionType == 3) { 
                    var ans = _context.Answers.Where(a => a.QuestionId == questionId).ToList();
                    result.AddRange(ans);

                }
                else
                {
                    var a = _context.Answers.Where(a => a.QuestionId == questionId).FirstOrDefault();
                    result.Add(a);
                }
            }else { 
                
                if(sectionType == 8)
                {
                    var ans = _context.Answers.Where(a => a.QuestionId == questionId && a.TypeCorrect == 1).ToList();
                    result.AddRange(ans);
                }
                else
                {
                    var a = _context.Answers.Where(a => a.QuestionId == questionId).FirstOrDefault();
                    result.Add(a);
                }
            }

            return result;
        }

        public async Task SaveTestResultAsync(TestResult testResult)
        {
            await _context.TestResult.AddAsync(testResult);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalQuestionBySkillId(Guid skillId)
        {
            return await _context.Skills
                .Where(skill => skill.Id == skillId)
                .SelectMany(skill => skill.Parts) // Get all parts of the skill
                .SelectMany(part => part.Sections) // Get all sections of each part
                .SelectMany(section => section.SectionQuestions) // Get all section questions
                .CountAsync(); // Count all questions
        }


        public async Task<int> GetAttemptCountByTestAndUserAsync(Guid userId, Guid testId)
        {
            // Count how many attempts the user has made for this test
            var attemptCount = await _context.TestResult
                .Where(tr => tr.UserId == userId && tr.TestId == testId)
                .CountAsync();

            return attemptCount;
        }
        public async Task UpdateExplainQuestionAsync(Guid questionId, string? explain)
        {
            // Retrieve the SectionQuestion entity and include the related Question
            var sectionQuestion = await _context.SectionQuestion
                .Include(sq => sq.Question)  // Explicitly include the Question navigation property
                .Where(sq => sq.Question.Id == questionId) // Filter by Question Id
                .FirstOrDefaultAsync();

            if (sectionQuestion == null)
            {
                throw new KeyNotFoundException($"SectionQuestion with QuestionId {questionId} not found.");
            }

            // Update the Explain property
            sectionQuestion.Explain = explain;

            // Save changes to the database
            await _context.SaveChangesAsync();
        }
        public async Task AddAttemptTestForYear(Guid userId, int year)
        {
            var testAttempt = await _context.AttempTests
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Year == year);

            if (testAttempt != null)
            {
                testAttempt.TotalAttempt += 1;
            }
            else
            {
                testAttempt = new AttempTest
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Year = year,
                    TotalAttempt = 1
                };
                await _context.AttempTests.AddAsync(testAttempt); // Add the new record
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        public async Task<List<AttempTest>> GetAttemptTests(Guid userId)
        {
            return await _context.AttempTests
                .Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task CreateSkillsAsync(Guid userId, Guid testId, Dictionary<string, SkillDto> model)
        {
            if (model == null || !model.Any())
                throw new ArgumentException("Model cannot be null or empty.", nameof(model));

            var executionStrategy = _context.Database.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    foreach (var skillKeyValue in model)
                    {
                        var skillDto = skillKeyValue.Value;


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

                        if (skillDto.Parts != null && skillDto.Parts.Any())
                        {
                            int partIndex = 1;
                            int questionOrder = 1;

                            foreach (var partDto in skillDto.Parts)
                            {
                                if (partDto == null)
                                    continue;

                                var part = new Part
                                {
                                    Id = Guid.NewGuid(),
                                    PartNumber = partIndex,
                                    ContentText = partDto.ContentText ?? "",
                                    Audio = partDto.Audio ?? "",
                                    Image = partDto.Image ?? "",
                                    Skill = skill,
                                    Sections = new List<Section>()
                                };

                                if (skill.Type == 1 && !string.IsNullOrWhiteSpace(partDto.Audio))
                                {
                                    part.AudioProcessingStatus = 0;
                                    var partId = part.Id;
                                    Task.Run(async () =>
                                    {
                                        using (var scope = _serviceProvider.CreateScope()) // Create a new scope
                                        {
                                            var scopedContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // Get a new instance of DbContext
                                            string audioText = await _azureService.ProcessAndTranscribeAudioCreateInSkills(partDto.Audio);
                                            Console.WriteLine("Test convert ended");

                                            await UpdatePartContentTextAsync(scopedContext, partId, audioText);
                                            Console.WriteLine("Convert successfully.");
                                        }
                                    });


                                }

                                int sectionOrder = 1;
                                if (partDto.Sections != null && partDto.Sections.Any())
                                {
                                    foreach (var sectionDto in partDto.Sections)
                                    {
                                        if (sectionDto == null)
                                            continue;

                                        var readingCondition = skillDto.Type == SkillTypeEnum.Reading
                                                 && (sectionDto.SectionType == 7
                                                  || sectionDto.SectionType == 8
                                                  || sectionDto.SectionType == 9
                                                  || sectionDto.SectionType == 10
                                                  || sectionDto.SectionType == 11);

                                        var listeningCondition = skillDto.Type == SkillTypeEnum.Listening
                                               && (sectionDto.SectionType == 1
                                                || sectionDto.SectionType == 2
                                                || sectionDto.SectionType == 3
                                                || sectionDto.SectionType == 4
                                                || sectionDto.SectionType == 7);



                                        var section = new Section
                                        {
                                            Id = Guid.NewGuid(),
                                            SectionGuide = sectionDto.SectionGuide,
                                            SectionType = sectionDto.SectionType,
                                            SectionOrder = sectionOrder,
                                            Image = sectionDto.Image,
                                            Explain = sectionDto.Explain,
                                            SectionContext = sectionDto.SectionContext,
                                            Part = part,
                                            SectionQuestions = new List<SectionQuestion>()
                                        };


                                        if (sectionDto.Questions != null && sectionDto.Questions.Any())
                                        {

                                            foreach (var questionDto in sectionDto.Questions)
                                            {
                                                if (questionDto == null)
                                                    continue;
                                                Question question;
                                                if (questionDto.IsFromQuestionBank == true)
                                                {

                                                    question = _context.Questions
                                                              .FirstOrDefault(q => q.Id == questionDto.QuestionId);
                                                    if (question == null)
                                                        continue;
                                                }
                                                else if (readingCondition || listeningCondition)
                                                {
                                                    question = new Question
                                                    {
                                                        Id = questionDto.QuestionId,
                                                        UserId = userId,
                                                        QuestionName = questionDto.QuestionName ?? "",
                                                        Explain = questionDto.Explain ?? "",
                                                        QuestionType = section.SectionType,
                                                        Skill = (int)skillDto.Type,
                                                        PartNumber = part.PartNumber,
                                                        Answers = new List<Answer>()
                                                    };
                                                    question.Answers.Add(new Answer()
                                                    {
                                                        Id = new Guid(),
                                                        AnswerText = questionDto.Answer,
                                                        TypeCorrect = 1,
                                                    });

                                                }
                                                else
                                                {
                                                    question = new Question
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        UserId = userId,
                                                        QuestionName = questionDto.QuestionName ?? "",
                                                        Explain = questionDto.Explain ?? "",
                                                        QuestionType = section.SectionType,
                                                        Skill = (int)skillDto.Type,
                                                        PartNumber = part.PartNumber,
                                                        Answers = new List<Answer>()
                                                    };

                                                    if (questionDto.Answers.Any())
                                                    {
                                                        foreach (var answerDto in questionDto.Answers)
                                                        {
                                                            if (answerDto == null)
                                                                continue;

                                                            var newAnswer = new Answer
                                                            {
                                                                Id = Guid.NewGuid(),
                                                                AnswerText = answerDto.AnswerText,
                                                                TypeCorrect = (int)answerDto.IsCorrect
                                                            };
                                                            question.Answers.Add(newAnswer);
                                                        }
                                                    }
                                                }

                                                var sectionQuestion = new SectionQuestion
                                                {
                                                    Id = Guid.NewGuid(),
                                                    Section = section,
                                                    Question = question,
                                                    Explain = questionDto.Explain ?? "",
                                                    QuestionOrder = questionOrder,
                                                };

                                                section.SectionQuestions.Add(sectionQuestion);
                                                questionOrder++;
                                            }
                                        }

                                        part.Sections.Add(section);
                                        sectionOrder++;
                                    }
                                }

                                partIndex++;
                                skill.Parts.Add(part);
                            }
                        }

                        await _context.Skills.AddAsync(skill);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        private async Task UpdatePartContentTextAsync(AppDbContext scopedContext, Guid partId, string audioText)
        {
            try
            {
                var part = await scopedContext.Parts.FirstOrDefaultAsync(p => p.Id == partId);

                if (part == null)
                {
                    Console.WriteLine($"Part with Id {partId} not found.");
                    return;
                }

                part.ScriptAudio = audioText;
                part.AudioProcessingStatus = 1;

                scopedContext.Parts.Update(part);

                // Save changes to the database
                await scopedContext .SaveChangesAsync();
                Console.WriteLine($"Successfully updated part with Id {partId}.");
            }
            catch (DbUpdateException dbEx)
            {
                // Log specific database-related errors
                Console.WriteLine($"Database update error: {dbEx.Message}");
                Console.WriteLine($"Stack Trace: {dbEx.StackTrace}");
            }
            catch (Exception ex)
            {
                // Log any other general errors
                Console.WriteLine($"Error updating part with Id {partId}: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        public async Task<List<Answer>> GetAnswerByQuestionId(Guid questionId)
        {
            return await _context.Answers
                    .Where(answer => answer.QuestionId == questionId).ToListAsync();
        }

        private List<Question> ParseSummaryToQuestions(string summary, Guid userId)
        {
            var questions = new List<Question>();

            // Regular expression to match sections of text containing placeholders in square brackets
            var pattern = @"(.*?)(\[(.*?)\])(.*?)(\.|$)";
            var matches = Regex.Matches(summary, pattern);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    var beforePlaceholder = match.Groups[1].Value.Trim();
                    var answerText = match.Groups[3].Value.Trim();
                    var afterPlaceholder = match.Groups[4].Value.Trim();

                    var questionText = $"{beforePlaceholder} [] {afterPlaceholder}".Trim();

                    var question = new Question
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        QuestionName = questionText,
                        QuestionType = 1,
                        Answers = new List<Answer>
                {
                    new Answer { Id = Guid.NewGuid(), AnswerText = answerText, TypeCorrect = 1 }
                }
                    };

                    questions.Add(question);
                }
            }

            return questions;
        }

        public async Task<bool> CheckExistedName(Guid userId, string testName)
        {
            return await _context.TestExams
                .AnyAsync(t => t.UserID == userId && t.TestName.ToLower() == testName.ToLower());
        }

        public async Task<TestModel> AddTestAsync(Guid userId, TestModel model, int role)
        {
            var newTest = new TestExam
            {
                Id = Guid.NewGuid(),
                TestName = model.TestName,
                StartTime = model.StartTime,
                TestType = model.TestType,
                EndTime = model.EndTime,
                CreateAt = DateTime.UtcNow,
                UpdateAt = DateTime.UtcNow,
                UserID = userId,
                TestCreateBy = role,
            };
           
            if (model.LessonId != Guid.Empty && model.LessonId != null)
            {
                newTest.LessonId = model.LessonId.Value;
            }
            if (model.SkilLIdCourse != Guid.Empty && model.SkilLIdCourse != null)
            {
                newTest.SkillIdCourse = model.SkilLIdCourse.Value;
            }
            if (model.CourseId != Guid.Empty && model.CourseId != null)
            {
                newTest.CourseId = model.CourseId.Value;
            }
            
            if(model.ClassId != Guid.Empty && model.ClassId != null)
            {
                    var classRelation = new ClassRelationShip
                    {
                        Id = Guid.NewGuid(),
                        TestId = newTest.Id,
                        ClassId = (Guid)model.ClassId
                    };

                    _context.ClassRelationShip.Add(classRelation);
            }
         
            _context.TestExams.Add(newTest);

            await _context.SaveChangesAsync();

            model.Id = newTest.Id;
            return model;
        }

        public async Task<List<Question>> GetQuestionsBySecionTypeAsync(Guid userId, int skill, int sectionType, int page, int pageSize)
        {
            // Calculate the skip count based on page and pageSize
            int skip = (page - 1) * pageSize;

            return await _context.Questions
                                 .Where(q => q.UserId == userId && q.Skill == skill && q.QuestionType == sectionType)
                                 .Include(q => q.Answers)
                                 .Skip(skip)  // Skip the previous pages' items
                                 .Take(pageSize)  // Take the next 'pageSize' items
                                 .ToListAsync();
        }


        public async Task<List<Question>> GetQuestionsAsync(Guid userId, int page, int pageSize)
        {
            // Calculate the skip count based on page and pageSize
            int skip = (page - 1) * pageSize;

            return await _context.Questions
                                 .Where(q => q.UserId == userId)
                                 .Include(q => q.Answers)
                                 .Skip(skip)  // Skip the previous pages' items
                                 .Take(pageSize)  // Take the next 'pageSize' items
                                 .ToListAsync();
        }

        public async Task<List<TestResult>> GetTestSubmittedAsync(Guid userId, int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            var testIds = await _context.TestExams
                 .Where(test => test.UserID == userId)
                 .Select(test => test.Id)
                 .ToListAsync();


            var results = await _context.TestResult
           .Where(test => testIds.Contains(test.TestId))
           .OrderByDescending(test => test.TestDate) // Order by most recent test date
           .Skip((page - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync();
            return results;
        }

        public async Task<object> GetTestAnalysisAttempt(Guid userId)
        {
            var results = await _context.TestResult
                .Where(test => test.UserId == userId)
                .GroupBy(test => test.TestDate.Date) // Group by date, ignoring time
                .Select(group => new
                {
                    TestDate = group.Key, // Grouped by the date
                    AttemptNumber = group.Count() // Count the number of tests in the group
                })
                .ToListAsync();

            var skillRada = await _context.TestResult
                .Where(test => test.UserId == userId) // Filter by UserId
                .GroupBy(test => test.SkillType)      // Group by SkillType
                .Select(group => new
                {
                    SkillType = group.Key, // The SkillType (integer)
                    Count = group.Count(), // Count the number of tests in the group
                    AverageScore = group.Average(t => t.Score) // Average score of tests in the group
                })
                .ToListAsync();

            return new
            {
                DateAnalysis = results,
                SkillAnalysis = skillRada
            };
        }


        public async Task AddQuestionsAsync(Question question)
        {
           
            foreach(var a in question.Answers)
                _context.Answers.Add(a);

            _context.Questions.Add(question); 
            await _context.SaveChangesAsync(); 
        }

        public async Task ImportQuestionAsync(List<Question> questions, Guid userId)
        {
            foreach (var question in questions)
            {
                // Create a new Question entity
                var newQuestion = new Question
                {
                    Id = Guid.NewGuid(),
                    QuestionName = question.QuestionName ?? string.Empty, // Default empty if null
                    QuestionType = question.QuestionType,
                    Skill = question.Skill,
                    PartNumber = question.PartNumber,
                    Explain = question.Explain ?? string.Empty, // Default empty if null
                    UserId = userId,
                    Answers = new List<Answer>() // Initialize the Answers collection
                };

                // Add answers if any
                if (question.Answers != null && question.Answers.Any())
                {
                    foreach (var answer in question.Answers)
                    {
                        if (!string.IsNullOrWhiteSpace(answer.AnswerText))
                        {
                            var newAnswer = new Answer
                            {
                                Id = Guid.NewGuid(),
                                QuestionId = newQuestion.Id,
                                AnswerText = answer.AnswerText,
                                TypeCorrect = answer.TypeCorrect ?? 0 // Default to 0 if null
                            };

                            _context.Answers.Add(newAnswer);
                            newQuestion.Answers.Add(newAnswer); // Associate with the question
                        }
                    }
                }

                _context.Questions.Add(newQuestion);
            }

            // Save changes with exception handling
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Database update error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}");
            }
        }


        public async Task<IEnumerable<TestExam>> GetAllTestsAsync(Guid userId)
        {
            return await _context.TestExams
                .Where(test => test.UserID == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TestExam>> GetPagedAdminTests(int pageNumber, int pageSize)
        {
            return await _context.TestExams
                .Where(test => test.TestCreateBy == 1)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalAdminTestsCount()
        {
            return await _context.TestExams
                .Where(test => test.TestCreateBy == 1)
                .CountAsync();
        }


        public async Task UpdateExplainSection(Guid sectionId, string explain)
        {
            var section = await _context.Sections.Where(sq => sq.Id == sectionId).FirstAsync();
            if(section != null)
            {
                section.Explain = explain;
            }
        }

        public async Task<List<Section>> GetSectionsByPartId(Guid partId)
        {
            return await _context.Sections.Where(s => s.Part.Id == partId).Include(s => s.SectionQuestions).ThenInclude(sq => sq.Question).ToListAsync();
        }

        public async Task<List<Skill>> GetSkillsExplainByTestIdAsync(Guid testId, List<int> totalParts)
        {
            return await _context.Skills
                .Where(s => s.TestId == testId)
                .Include(s => s.Parts.Where(p => totalParts.Contains(p.PartNumber)))  // Filter Parts by PartNumber
                    .ThenInclude(p => p.Sections)
                        .ThenInclude(sec => sec.SectionQuestions)
                            .ThenInclude(sq => sq.Question)
                                .ThenInclude(q => q.Answers)
                .ToListAsync();
        }

        public async Task<List<Skill>> GetSkillsByTestIdAsync(Guid testId)
        {
            return await _context.Skills
                .Where(s => s.TestId == testId)
                .Include(s => s.Parts)  // Filter Parts by PartNumber
                    .ThenInclude(p => p.Sections)
                        .ThenInclude(sec => sec.SectionQuestions)
                            .ThenInclude(sq => sq.Question)
                                .ThenInclude(q => q.Answers)
                .ToListAsync();
        }
        

        public async Task<List<UserAnswers>> GetUserAnswersByTestId(Guid testId, Guid userId)
        {
            return await _context.UserAnswers
                .Where(a => a.TestId == testId && a.UserId == userId).ToListAsync();
        }

        public async Task<Skill> GetSkillByIdAsync(Guid skillId)
        {  
            return await _context.Skills
                .Include(s => s.Parts)
                    .ThenInclude(p => p.Sections)
                        .ThenInclude(sec => sec.SectionQuestions)
                            .ThenInclude(sq => sq.Question)
                                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(s => s.Id == skillId);
        }

        public async Task<Skill> GetSkillExplainByIdAsync(Guid skillId, List<int> parts)
        {
            return await _context.Skills
                .Include(s => s.Parts.Where(p => parts.Contains(p.PartNumber)))
                    .ThenInclude(p => p.Sections)
                        .ThenInclude(sec => sec.SectionQuestions)
                            .ThenInclude(sq => sq.Question)
                                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(s => s.Id == skillId);
        }

        public async Task<List<Skill>> GetSkills(Guid testId)
        {
            return await _context.Skills.Where(skill => skill.TestId == testId).ToListAsync();
        }
        public async Task<List<Part>> GetParts(Guid skillId)
        {
            return await _context.Parts.Where(part => part.Skill.Id == skillId).OrderBy(part => part.PartNumber).ToListAsync();
        }

        public async Task<TestExam> GetTestAsync(Guid id)
        {
            return await _context.TestExams
                .FirstOrDefaultAsync(test => test.Id == id);
        }


        public async Task<List<TestResult>> GetResultTest(Guid userId, List<Guid> ids)
        {
            return await _context.TestResult
                .Where(test => ids.Contains(test.Id) && test.UserId == userId)
                .ToListAsync();
        }

        public async Task<Skill> GetSkillByIdNe(Guid skillId)
        {
            return await _context.Skills.Where(s => s.Id == skillId).FirstAsync();
        }
        public async Task<TestExam> GetTestBySecionCourseId(Guid id)
        {
            return await _context.TestExams
                .FirstOrDefaultAsync(test => test.LessonId == id);
        }

        public async Task<Question> GetQuestionByIdAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Answers) // Include answers for the question
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task UpdateQuestionAsync(Guid questionId,QuestionBankModel updatedQuestion)
        {
            // Retrieve the existing question from the database
            var existingQuestion = await _context.Questions
                .Include(q => q.Answers) // Include related answers
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (existingQuestion == null)
            {
                throw new ArgumentException($"Question with ID {questionId} does not exist.");
            }

            // Update the question's fields
            existingQuestion.QuestionName = updatedQuestion.QuestionName;
            existingQuestion.QuestionType = updatedQuestion.QuestionType;
            existingQuestion.Skill = updatedQuestion.SkillType;
            existingQuestion.PartNumber = updatedQuestion.Part;

            // Remove all existing answers
            var existingAnswers = existingQuestion.Answers.ToList();
            _context.Answers.RemoveRange(existingAnswers);

            // Check if there are new answers to add
            if (updatedQuestion.Answers != null && updatedQuestion.Answers.Count > 0)
            {
                // Add new answers from the front-end
                foreach (var updatedAnswerDto in updatedQuestion.Answers)
                {
                    var newAnswer = new Answer
                    {
                        AnswerText = updatedAnswerDto.AnswerText,
                        TypeCorrect = (int)updatedAnswerDto.IsCorrect,
                        QuestionId = existingQuestion.Id
                    };
                    existingQuestion.Answers.Add(newAnswer);
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
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

        public async Task<List<TestExam>> GetTestExamByLessonIdAsync(Guid lessonId)
        {
            return await _context.TestExams
                                 .Where(te => te.LessonId == lessonId)
                                 .ToListAsync();
        }
        public async Task<List<TestExam>> GetTestExamsBySkillIdCourseIdAsync(Guid skillId)
        {
            return await _context.TestExams
                                 .Where(te => te.SkillIdCourse == skillId)
                                 .Select(te => new TestExam
                                 {
                                     Id = te.Id,
                                     TestName = te.TestName
                                 })
                                 .ToListAsync();
        }

        public async Task<List<TestExam>> GetTestExamsByClassIdAsync(Guid classId)
        {
            // Fetch the TestIds associated with the ClassId
            var testIds = await _context.ClassRelationShip
                                        .Where(t => t.ClassId == classId)
                                        .Select(t => t.TestId)
                                        .ToListAsync();

            // Fetch TestExams that match the TestIds
            return await _context.TestExams
                                 .Where(te => testIds.Contains(te.Id)) // Assuming TestExam has an Id matching TestId
                                 .ToListAsync();
        }


        public async Task<(IEnumerable<TestExam> tests, int totalCount)> GetTestsAsync(int page, int pageSize)
        {
            if (page <= 0) throw new ArgumentException("Page number must be greater than 0", nameof(page));
            if (pageSize <= 0) throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

            var totalCount = await _context.TestExams.CountAsync();

            var tests = await _context.TestExams
                .OrderByDescending(t => t.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tests, totalCount);
        }

        public async Task<TestExam> UpdateTestAsync(TestExam testExam)
        {
            if (testExam == null) throw new ArgumentNullException(nameof(testExam));

            var existingTest = await _context.TestExams.FindAsync(testExam.Id);

            if (existingTest == null) throw new KeyNotFoundException("TestExam not found.");

            existingTest.TestName = testExam.TestName;
            existingTest.StartTime = testExam.StartTime;
            existingTest.EndTime = testExam.EndTime;
            existingTest.UpdateAt = DateTime.Now;

            _context.TestExams.Update(existingTest);

            await _context.SaveChangesAsync();

            return existingTest;
        }

        public async Task<bool> DeleteTestAsync(Guid id)
        {
            var test = await _context.TestExams.FirstOrDefaultAsync(p => p.Id == id);
            if (test != null)
            {
                _context.TestExams.Remove(test);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<(IEnumerable<TestExam> tests, int totalCount)> GetTestExamByCourseIdAsync(Guid courseId, int page, int pageSize)
        {
            if (page <= 0) throw new ArgumentException("Page number must be greater than 0", nameof(page));
            if (pageSize <= 0) throw new ArgumentException("Page size must be greater than 0", nameof(pageSize));

            var query = _context.TestExams.Where(t => t.CourseId == courseId);
            var totalCount = await query.CountAsync();

            var tests = await query
                .OrderByDescending(t => t.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (tests, totalCount);
        }

        public async Task<(IEnumerable<TestResultWithExamDto>, int)> GetTestResultByUserIdAsync(Guid courseId, string userId)
        {
            var query = _context.TestResult
                .Join(
                    _context.TestExams,
                    tr => tr.TestId,
                    te => te.Id,
                    (tr, te) => new { TestResult = tr, TestExam = te }
                )
                .Where(joined => joined.TestResult.UserId == Guid.Parse(userId)
                                 && joined.TestExam.CourseId == courseId)
                .Select(joined => new
                {
                    TestResult = joined.TestResult,
                    TestExam = joined.TestExam
                });

            var totalCount = await query.CountAsync();
            var testResults = await query.ToListAsync();

            // Project into a model that includes both TestResult and TestExam data
            var result = testResults.Select(item => new TestResultWithExamDto
            {
                Id = item.TestResult.Id,
                TestId = item.TestResult.TestId,
                UserId = item.TestResult.UserId,
                SkillType = item.TestResult.SkillType,
                Score = item.TestResult.Score,
                NumberOfCorrect = item.TestResult.NumberOfCorrect,
                TotalQuestion = item.TestResult.TotalQuestion,
                TestDate = item.TestResult.TestDate,
                TimeMinutesTaken = item.TestResult.TimeMinutesTaken,
                SecondMinutesTaken = item.TestResult.SecondMinutesTaken,
                AttemptNumber = item.TestResult.AttemptNumber,
                TestName = item.TestExam.TestName,
            });

            return (result, totalCount);
        }


        public async Task<(IEnumerable<TestResultWithExamDto> testResults, int totalCount)> GetTestResultOfTest(Guid testId)
        {
            var query = from tr in _context.TestResult
                        join te in _context.TestExams on tr.TestId equals te.Id
                        join user in _context.ApplicationUsers on tr.UserId.ToString() equals user.Id
                        where tr.TestId == testId
                        select new TestResultWithExamDto
                        {
                            Id = tr.Id,
                            TestId = tr.TestId,
                            UserId = tr.UserId,

                            SkillType = tr.SkillType,
                            Score = tr.Score,
                            NumberOfCorrect = tr.NumberOfCorrect,
                            TotalQuestion = tr.TotalQuestion,
                            TestDate = tr.TestDate,
                            TimeMinutesTaken = tr.TimeMinutesTaken,
                            SecondMinutesTaken = tr.SecondMinutesTaken,
                            AttemptNumber = tr.AttemptNumber,
                            TestName = te.TestName,
                            UserName = user.Name,
                            UserEmail = user.Email,
                            TotalParts = JsonConvert.DeserializeObject<List<int>>(tr.TotalParts)
                        };

            // Get total count
            var totalCount = await query.CountAsync();

            // Get test results
            var testResults = await query.ToListAsync();

            return (testResults, totalCount);
        }

    }
}
