using AutoMapper;
using Entity.Test;
using IRepository;
using IService;
using Microsoft.EntityFrameworkCore;
using Model.Test;
using Model.Utility;
using OfficeOpenXml.ConditionalFormatting.Contracts;
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

        public async Task<Dictionary<string, object>> GetExplainByTestId(TestExplainRequestDto model)
        {
            var result = new Dictionary<string, object>();

            List<Skill> sortedSkills;

            // Determine the skills to process
            if (model.SkillId != null && model.SkillId != Guid.Empty)
            {
                var skill = await _testExamRepository.GetSkillByIdAsync(model.SkillId.Value);
                if (skill == null) return result;
                sortedSkills = new List<Skill> { skill };
            }
            else
            {
                var skills = await _testExamRepository.GetSkillsByTestIdAsync(model.TestId);
                if (skills == null || !skills.Any()) return result; // No skills found
                sortedSkills = skills.OrderBy(skill => skill.Type).ToList();
            }

            // Fetch user answers
            var userAnswers = await _testExamRepository.GetUserAnswersByTestId(model.TestId, model.UserId);

            // Process skills
            foreach (var skill in sortedSkills)
            {
                string skillTypeKey = skill.Type switch
                {
                    0 => "reading",
                    1 => "listening",
                    2 => "writing",
                    3 => "speaking",
                    _ => "unknown"
                };

                result[skillTypeKey] = new
                {
                    id = skill.Id,
                    duration = skill.Duration,
                    type = skill.Type,
                    parts = skill.Parts.Select(part => new
                    {
                        id = part.Id,
                        partNumber = part.PartNumber,
                        contentText = part.ContentText,
                        audio = part.Audio,
                        image = part.Image,
                        sections = part.Sections.Select(section => new
                        {
                            id = section.Id,
                            sectionGuide = section.SectionGuide,
                            sectionType = section.SectionType,
                            image = section.Image,
                            questions = section.SectionQuestions.Select(sq =>
                            {
                                List<object> filteredUserAnswers;
                                if (skill.Type == 0 && section.SectionType == 1)
                                {

                                    var maxAttemptNumber = userAnswers
                                          .Where(ua => ua.QuestionId == sq.Question.Id)
                                          .Max(ua => ua.AttemptNumber);

                                    filteredUserAnswers = userAnswers
                                        .Where(ua => ua.QuestionId == sq.Question.Id && ua.AttemptNumber == maxAttemptNumber)
                                        .Select(ua => new
                                        {
                                            id = ua.Id,
                                            answerText = ua.AnswerText,
                                            answerId = ua.AnswerId,
                                            isCorrect = 0, // Placeholder for correctness logic
                                            attemptNumber = ua.AttemptNumber
                                        })
                                        .ToList<object>();
                                }
                                else
                                {
                                    filteredUserAnswers = userAnswers
                                         .Where(ua => ua.QuestionId == sq.Question.Id)
                                         .GroupBy(ua => ua.QuestionId)
                                         .Select(g => g.OrderByDescending(ua => ua.AttemptNumber).First())
                                         .Select(ua => new
                                         {
                                             id = ua.Id,
                                             answerText = ua.AnswerText,
                                             isCorrect = 0, // Placeholder for correctness logic
                                             attemptNumber = ua.AttemptNumber
                                         })
                                         .ToList<object>();
                                }

                                return new
                                {
                                    id = sq.Question.Id,
                                    questionName = sq.Question.QuestionName,
                                    answers = sq.Question.Answers?.Select(ans => new
                                    {
                                        id = ans.Id,
                                        answerText = ans.AnswerText,
                                        isCorrect = ans.TypeCorrect
                                    }).ToList(),
                                    userAnswers = filteredUserAnswers // Mảng userAnswers
                                };
                            }).ToList()
                        }).ToList()
                    }).ToList()
                };
            }

            return result;
        }


        public async Task<TestResult> CalculateScore(Guid testId, Guid userId, SubmitTestDto model)
        {
            var totalQuestion = await _testExamRepository.GetTotalQuestionBySkillId(model.UserAnswers.Values.First().SkillId);
            var userAnswers = new List<UserAnswers>();
            decimal totalScore = 0;
            int totalCorrectAnswer = 0;

            int attemptNumber = await _testExamRepository.GetAttemptCountByTestAndUserAsync(userId, testId);

            foreach (var entry in model.UserAnswers)
            {
                var questionDetail = entry.Value;

                bool isCorrect = await ValidateAnswer(questionDetail.QuestionId, questionDetail.Answers, questionDetail.SectionType, questionDetail.Skill);

                decimal questionScore = isCorrect ? 1 : 0;
                int correctQuestion = isCorrect ? 1 : 0;
                totalScore += questionScore;
                totalCorrectAnswer += correctQuestion;
                foreach (var answer in questionDetail.Answers)
                {
                    var userAnswer = new UserAnswers
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        QuestionId = questionDetail.QuestionId,
                        TestId = testId,
                        AnswerText = answer.AnswerText,
                        AnswerId = answer.AnswerId.HasValue ? answer.AnswerId.Value : (Guid?)null,
                        AttemptNumber = attemptNumber
                    };
                    userAnswers.Add(userAnswer);

                }

            }

            await _testExamRepository.SaveUserAnswerAsync(userAnswers);

            var testResult = new TestResult
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TestId = testId,
                SkillType = model.UserAnswers.Values.First().Skill,
                Score = ScaleScore(totalScore, totalQuestion),
                NumberOfCorrect = totalCorrectAnswer,
                TotalQuestion = totalQuestion,
                TestDate = DateTime.UtcNow,
                TimeMinutesTaken = model.TimeMinutesTaken,
                SecondMinutesTaken = model.TimeSecondsTaken,
                AttemptNumber = attemptNumber
            };

            // Save test result using repository
            await _testExamRepository.SaveTestResultAsync(testResult);

            return testResult;
        }

        private async Task<bool> ValidateAnswer(Guid questionId, List<AnswerDto> usewrAnswers, int sectionType, int skill)
        {
            var correctAnswers = await _testExamRepository.GetAnswerByQuestionId(questionId);

            if (correctAnswers == null)
            {
                Console.WriteLine($"No correct answer found for question ID: {questionId}");
                return false;
            }

            try
            {
                switch (skill)
                {
                    case 0:
                        if (sectionType == 2 )
                        {
                            if (int.TryParse(usewrAnswers[0].AnswerText, out int userAnswerNumber))
                            {
                                return userAnswerNumber == correctAnswers[0].TypeCorrect;
                            }
                        }
                        else if (sectionType == 1)
                            return CompareMutipleAnswerSets(correctAnswers, usewrAnswers);
                        else if(sectionType == 4 || sectionType== 5 || sectionType == 6)
                                return correctAnswers[0].Id == usewrAnswers[0].AnswerId;
                        else
                            return correctAnswers[0].AnswerText == usewrAnswers[0].AnswerText;
                        break;
                    case 1:
                        if (sectionType == 8)
                            return CompareMutipleAnswerSets(correctAnswers, usewrAnswers);
                        else if (sectionType == 1)
                            return true;
                        else
                            return correctAnswers[0].AnswerText == usewrAnswers[0].AnswerText;
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating answer for question ID: {questionId}, Error: {ex.Message}");
            }

            return false;
        }

        private bool CompareMutipleAnswerSets(List<Answer> correctAnswers, List<AnswerDto> userAnswers)
        {
            var correctIds = correctAnswers
                             .Where(a => a.TypeCorrect == 1)
                             .Select(a => a.Id)
                             .ToHashSet();

            var userAnswerIds = userAnswers
                                .Select(ua => ua.AnswerId.GetValueOrDefault())
                                .ToHashSet();

            return correctIds.SetEquals(userAnswerIds);
        }


        private decimal ScaleScore(decimal rawScore, int totalQuestion)
        {
            if (totalQuestion == 0) return 0; // Prevent division by zero
            return Math.Round((rawScore / totalQuestion) * 9, 2);
        }

        public async Task<TestModel> CreateTestAsync(Guid userId, TestModel model, string userRoleClaim)
        {
            int role = userRoleClaim.Equals(SD.Teacher) ? 0 : 1;
            return await _testExamRepository.AddTestAsync(userId, model, role);
        }

    }
}
