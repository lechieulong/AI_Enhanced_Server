using AutoMapper;
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
        private readonly IMapper _mapper;

        public TestExamService(ITestExamRepository testExamRepository)
        {
            _testExamRepository = testExamRepository;
        }


        //public async Task CreateSkillsAsync(Guid userId, Guid testId, Dictionary<string, SkillDto> model)
        //{
        //    foreach (var skillKeyValue in model)
        //    {
        //        var skillDto = skillKeyValue.Value;

        //        var skill = new Skill
        //        {
        //            Id = Guid.NewGuid(),
        //            TestId = testId,
        //            Duration = skillDto.Duration,
        //            Type = (int)skillDto.Type
        //        };

        //        int partIndex = 1;
        //        foreach (var partDto in skillDto.Parts)
        //        {
        //            // Manually create Part entity without mapping
        //            var part = new Part
        //            {
        //                Id = Guid.NewGuid(),
        //                PartNumber = partIndex,
        //                ContentText = partDto.ContentText,
        //                Audio = partDto.Audio,
        //                Image = partDto.Image,
        //                Skill = skill,
        //                Sections = new List<Section>() // Initialize Sections
        //            };

        //            foreach (var sectionDto in partDto.Sections)
        //            {
        //                // Manually create Section entity without mapping
        //                var section = new Section
        //                {
        //                    Id = Guid.NewGuid(),
        //                    SectionGuide = sectionDto.SectionGuide,
        //                    SectionType = sectionDto.SectionType,
        //                    Part = part,
        //                    SectionQuestions = new List<SectionQuestion>() // Initialize SectionQuestions
        //                };

        //                foreach (var questionDto in sectionDto.Questions)
        //                {
        //                    // Manually create Question entity without mapping
        //                    var question = new Question
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        UserId = userId,
        //                        QuestionName = questionDto.QuestionName,
        //                        Answers = new List<Answer>() 
        //                    };

        //                    // Manually map answers from QuestionDto to Question entity
        //                    question.Answers = questionDto.Answers.Select(answerDto =>
        //                    {
        //                        return new Answer
        //                        {
        //                            Id = Guid.NewGuid(),
        //                            AnswerText = answerDto.AnswerText,
        //                            TypeCorrect = (int)answerDto.TypeCorrect,
        //                            QuestionId = question.Id,
        //                            Question = question
        //                        };
        //                    }).ToList();

        //                    // Link the question to the section through SectionQuestion
        //                    var sectionQuestion = new SectionQuestion
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        Section = section,
        //                        Question = question,
        //                        SectionId = section.Id,
        //                        QuestionId = question.Id
        //                    };

        //                    section.SectionQuestions.Add(sectionQuestion);
        //                }

        //                // Add section to part
        //                part.Sections.Add(section);
        //            }

        //            partIndex++;
        //            skill.Parts.Add(part);
        //        }

        //        // Persist the skill with all its related entities
        //        await _testExamRepository.AddSkillAsync(skill);
        //    }
        //}

        public async Task<TestModel> CreateTestAsync(Guid userId,TestModel model)
        {
            return await _testExamRepository.AddTestAsync(userId,model);
        }
      
    }
}
