﻿using AutoMapper;
using Entity.Test;
using IRepository;
using IService;
using Microsoft.EntityFrameworkCore;
using Model.Test;
using Model.Utility;
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

        public async Task<TestResult> CalculateScore(Guid testId, Guid userId,  Dictionary<string, UserAnswersDto> payload)
        {
            var totalQuestion = await _testExamRepository.GetTotalQuestionBySkillId(payload.Values.First().SkillId);
            var userAnswers = new List<UserAnswers>();
            decimal totalScore = 0;
            int totalCorrectAnswer = 0;

            foreach (var entry in payload)
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
                SkillType = payload.Values.First().Skill,
                Score = ScaleScore(totalScore, totalQuestion), 
                NumberOfCorrect = totalCorrectAnswer,
                TotalQuestion = totalQuestion

            };

            // Save test result using repository
            await _testExamRepository.SaveTestResultAsync(testResult);

            return testResult;
        }

        private async Task<bool> ValidateAnswer(Guid questionId, List<AnswerDto> answers, int sectionType, int skill)
        {
            var correctAnswer =  await _testExamRepository.GetAnswerByQuestionId(questionId);

            if (correctAnswer == null)
            {
                Console.WriteLine($"No correct answer found for question ID: {questionId}");
                return false;
            }

            try
            {
                switch (skill)
                {
                    case 0:
                        if (sectionType == 2)
                        {
                            if (int.TryParse(answers[0].AnswerText, out int userAnswerNumber))
                            {
                                return userAnswerNumber == correctAnswer.TypeCorrect;
                            }
                        }
                        break;
                    case 1: 
                        break;
                  
                }
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors and log them
                Console.WriteLine($"Error validating answer for question ID: {questionId}, Error: {ex.Message}");
            }

            return false;
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
