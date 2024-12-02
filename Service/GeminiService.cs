﻿using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System.Text;
using IService;
using Entity.Test;
using System.CodeDom;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Model.Test;
using IRepository;
using System.Text.RegularExpressions;

namespace Service
{
    public class GeminiService : IGeminiService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ITestExamRepository _testExamRepository;

        private readonly string _apiUrl;

        public GeminiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ITestExamRepository testExamRepository)
        {
            _clientFactory = httpClientFactory;
            _testExamRepository = testExamRepository;
            _apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key=AIzaSyAkFBzNmOixBE8Elh-nNseThbJZMJAMc_A";
        }

        public async Task<SubmitTestDto> ScoreAndExplain(SubmitTestDto model)
        {
            var client = _clientFactory.CreateClient();

            foreach (var userAnswer in model.UserAnswers.Values)
            {
                var questionName = await _testExamRepository.GetQuestionNameById(userAnswer.QuestionId);
                var requestData = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new { text = BuildPrompt(questionName, userAnswer.Answers[0].AnswerText,2) }
                    }
                }
                 }
                };

                var response = await client.PostAsync(
                    _apiUrl,
                    new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json")
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"API call failed with status code: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var aiResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                // Extract Overall Score and Feedback
                var result = aiResponse?.candidates[0]?.content?.parts[0]?.text?.ToString();

                if (!string.IsNullOrEmpty(result))
                {
                    // Use a regular expression to extract the Overall Score
                    var scoreMatch = Regex.Match(result, @"\*\*Overall Score:\*\*\s*([\d\.]+)");
                    if (scoreMatch.Success)
                    {
                        var overallScore = scoreMatch.Groups[1].Value;
                        userAnswer.OverallScore = overallScore;
                    }

                    // Extract Feedback
                    var feedbackMatch = Regex.Match(result, @"\*\*Feedback:\*\*\s*([\s\S]+?)\*\*Suggestions for Improvement:\*\*");
                    string feedback = string.Empty;
                    if (feedbackMatch.Success)
                    {
                        feedback = feedbackMatch.Groups[1].Value.Trim();
                    }

                    // Extract Suggestions for Improvement
                    var suggestionsMatch = Regex.Match(result, @"\*\*Suggestions for Improvement:\*\*\s*([\s\S]+)");
                    string suggestions = string.Empty;
                    if (suggestionsMatch.Success)
                    {
                        suggestions = suggestionsMatch.Groups[1].Value.Trim();
                    }

                    // Concatenate Feedback and Suggestions into Explain field
                    userAnswer.Explain = $"{feedback}\n\nSuggestions for Improvement:\n{suggestions}";
                }


            }

            return model;
        }


       

        private string BuildPrompt(string questionName, string answer, int task)
        {
            return $@"
IELTS Writing Task {task} Question: {questionName}
User's Response: {answer}

Please evaluate the response based on the following criteria:

1. **Task Relevance:**
   - Does the response directly and fully address the question?
   - If not, how much of the response is off-topic or irrelevant? (This is the most important factor in determining the score.)

2. **Task Response:**
   - Does the response provide clear arguments and relevant examples to support its points?
   - Is the argument sufficiently developed?

3. **Coherence and Cohesion:**
   - Is the essay well-structured, with a clear introduction, body paragraphs, and conclusion?
   - Are ideas logically organized, with appropriate transitions and cohesive devices?

4. **Lexical Resource:**
   - Is the vocabulary varied and used accurately?
   - Are there any errors in word choice, spelling, or repetition?

5. **Grammatical Range and Accuracy:**
   - Are a variety of sentence structures used effectively?
   - Are there any significant grammatical errors, such as incorrect tense, article usage, or sentence fragments?

### Evaluation Format:
- **Overall Score:** [Numeric score only, e.g., 3.0, 6.5, 7.5]
- **Feedback:** 
  - Task Relevance: [Your feedback]
  - Task Response: [Your feedback]
  - Coherence: [Your feedback]
  - Lexical Resource: [Your feedback]
  - Grammar: [Your feedback]
- **Suggestions for Improvement:** [Provide actionable tips for improvement]

Now, please evaluate the user's response based on the criteria above. Remember, Task Relevance is the most important factor when determining the score. If the answer doesn't directly address the question, the overall score should be lowered accordingly.
";
        }


        public async Task<SubmitTestDto> ScoreAndExplainSpeaking(SubmitTestDto model)
        {
            var client = _clientFactory.CreateClient();

            foreach (var userAnswer in model.UserAnswers.Values)
            {
                var questionName = await _testExamRepository.GetQuestionNameById(userAnswer.QuestionId);
                var requestData = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new { text = BuildPromptSpeaking(questionName, userAnswer.Answers[0].AnswerText,2) }
                    }
                }
                 }
                };

                var response = await client.PostAsync(
                    _apiUrl,
                    new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json")
                );

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"API call failed with status code: {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var aiResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                // Extract Overall Score and Feedback
                var result = aiResponse?.candidates[0]?.content?.parts[0]?.text?.ToString();

                if (!string.IsNullOrEmpty(result))
                {
                    // Use a regular expression to extract the Overall Score
                    var scoreMatch = Regex.Match(result, @"\*\*Overall Score:\*\*\s*([\d\.]+)");
                    if (scoreMatch.Success)
                    {
                        var overallScore = scoreMatch.Groups[1].Value;
                        userAnswer.OverallScore = overallScore;
                    }

                    // Extract Feedback
                    var feedbackMatch = Regex.Match(result, @"\*\*Feedback:\*\*\s*([\s\S]+?)\*\*Suggestions for Improvement:\*\*");
                    string feedback = string.Empty;
                    if (feedbackMatch.Success)
                    {
                        feedback = feedbackMatch.Groups[1].Value.Trim();
                    }

                    // Extract Suggestions for Improvement
                    var suggestionsMatch = Regex.Match(result, @"\*\*Suggestions for Improvement:\*\*\s*([\s\S]+)");
                    string suggestions = string.Empty;
                    if (suggestionsMatch.Success)
                    {
                        suggestions = suggestionsMatch.Groups[1].Value.Trim();
                    }

                    // Concatenate Feedback and Suggestions into Explain field
                    userAnswer.Explain = $"{feedback}\n\nSuggestions for Improvement:\n{suggestions}";
                }


            }

            return model;
        }


        private string BuildPromptSpeaking(string questionName, string answer, int part)
        {
            return $@"
### IELTS Speaking Part {part} Question:
**Question:** {questionName}

**User's Response:** 
{answer}

Please evaluate the response based on the following criteria:

1. **Task Relevance:**
   - Does the response directly and fully address the question?
   - How much of the response is off-topic or irrelevant? This is the most important factor when determining the score.

2. **Task Response:**
   - Does the response provide clear arguments and relevant examples to support its points?
   - Are the arguments well-developed and logically presented?

3. **Coherence and Cohesion:**
   - Is the response well-structured, with a clear introduction, body, and conclusion (if applicable)?
   - Are ideas logically organized, and is there a clear flow between ideas and sentences?
   - Are appropriate transition words and cohesive devices used?

4. **Lexical Resource:**
   - Is the vocabulary varied and used accurately?
   - Are there any errors in word choice, spelling, or repetition? Are more sophisticated words used where possible?

5. **Grammatical Range and Accuracy:**
   - Are a variety of sentence structures used effectively?
   - Are there any significant grammatical errors (e.g., tense errors, incorrect article usage, sentence fragments)?

### Evaluation Format:
- **Overall Score:** [Numeric score only, e.g., 3.0, 6.5, 7.5]
- **Feedback:**
  - **Task Relevance:** [Provide feedback here]
  - **Task Response:** [Provide feedback here]
  - **Coherence and Cohesion:** [Provide feedback here]
  - **Lexical Resource:** [Provide feedback here]
  - **Grammatical Range and Accuracy:** [Provide feedback here]
  
- **Suggestions for Improvement:** 
  - [Provide actionable and specific tips for improvement]

### Important Notes:
- Task Relevance is the most important factor in determining the score. If the response does not directly address the question, the overall score should be lowered accordingly.
- Make sure the feedback is constructive and detailed for the user to improve.
";
        }

    }
}
