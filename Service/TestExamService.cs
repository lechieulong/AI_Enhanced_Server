using Entity.Test;
using IRepository;
using IService;
using Model.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service
{
    public class TestExamService : ITestExamService
    {
        private readonly ITestExamRepository _testExamRepository;

        public TestExamService(ITestExamRepository testExamRepository)
        {
            _testExamRepository = testExamRepository;
        }


        public async Task CreateSkillsAsync(Guid testId, Dictionary<string, SkillDto> model)
        {
            foreach (var skillKeyValue in model)
            {
                var skillDto = skillKeyValue.Value;

                var skill = new Skill
                {
                    Id = Guid.NewGuid(),
                    Type = (int)skillDto.Type,
                    Duration = skillDto.Duration,
                    TestId = testId
                };

                foreach (var partDto in skillDto.Parts)
                {
                    var part = new Part
                    {
                        Id = Guid.NewGuid(),
                        ContentText = partDto.ContentText,
                        Audio = partDto.Audio,
                        Skill = skill
                    };

                    foreach (var sectionDto in partDto.Sections)
                    {
                        var section = new Section
                        {
                            Id = Guid.NewGuid(),
                            SectionGuide = sectionDto.SectionGuide,
                            SectionType = sectionDto.Type,
                            Image = sectionDto.Image,
                            Part = part
                        };

                        foreach (var questionDto in sectionDto.Questions)
                        {
                            var question = new Question
                            {
                                Id = Guid.NewGuid(),
                                QuestionName = questionDto.QuestionName,
                                QuestionType = (int)questionDto.QuestionType
                            };

                            var sectionQuestion = new SectionQuestion
                            {
                                Id = Guid.NewGuid(),
                                SectionId = section.Id,
                                QuestionId = question.Id
                            };

                            foreach (var answerDto in questionDto.Answers)
                            {
                                var answer = new Answer
                                {
                                    Id = Guid.NewGuid(),
                                    AnswerText = answerDto.AnswerText,
                                    IsCorrect = answerDto.IsCorrect,
                                    Question = question
                                };
                                question.Answers.Add(answer);
                            }
                            section.SectionQuestions.Add(sectionQuestion);
                        }

                        part.Sections.Add(section);
                    }

                    skill.Parts.Add(part);
                }

                await _testExamRepository.AddSkillAsync(skill);
            }
        }

        public async Task<TestModel> CreateTestAsync(TestModel model)
        {
            return await _testExamRepository.AddTestAsync(model);
        }
      
    }
}
