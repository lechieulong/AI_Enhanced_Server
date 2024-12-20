using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
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
using Common;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using Azure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.CognitiveServices.Speech.Transcription;
using Microsoft.Identity.Client;
using Microsoft.Rest.Azure;
using Microsoft.VisualBasic;
using NAudio.Codecs;
using static System.Formats.Asn1.AsnWriter;
using System.Data;
using System.Security.Principal;

namespace Service
{
    public class GeminiService : IGeminiService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ITestExamRepository _testExamRepository;
        private readonly IAzureService _azureService;

        private readonly string _apiUrl;

        public GeminiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ITestExamRepository testExamRepository, IAzureService azureService)
        {
            _clientFactory = httpClientFactory;
            _testExamRepository = testExamRepository;
            _apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key=AIzaSyAafcsKbt9C0XjKtLvGFQ5nG9IHm-sRtyU";
            _azureService = azureService;
        }

        public async Task<SubmitTestDto> ScoreAndExplain(SubmitTestDto model)
        {
            var client = _clientFactory.CreateClient();

            foreach (var userAnswer in model.UserAnswers.Values)
            {

                var part  = await _testExamRepository.GetPartNumber(userAnswer.PartId.Value);
                var questionName = await _testExamRepository.GetQuestionNameById(userAnswer.QuestionId);
                var descriptionDiagram = string.Empty;
                if (part.PartNumber == 1) {
                    descriptionDiagram = await _azureService.ExtractTextFromImageAsync(part.Image);
                    }

                var finalPrompt = part.PartNumber == 2 ? BuildPrompt(questionName, userAnswer.Answers[0].AnswerText, part.PartNumber) 
                                            : BuildPromptWithOCR(questionName,descriptionDiagram, userAnswer.Answers[0].AnswerText,part.PartNumber);

                var requestData = new
                {
                    contents = new[]
                    {
                new
                {
                    parts = new[]
                    {
                        new { text = finalPrompt}
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

        private string BuildPromptWithOCR(string questionName, string descriptionDiagram, string answer, int task)
        {
            return $@"
IELTS Writing Task {task}
Question: {questionName}
Description analysis diagram:  {descriptionDiagram}
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


        public async Task<SpeakingResponseDto> ScoreSpeaking(SpeakingModel speakingModel)
        {
            var speakingExplains = new SpeakingResponseDto();
            speakingExplains.Answer = speakingModel.Answer;
            speakingExplains.QuestionName = speakingModel.QuestionName;

            var client = _clientFactory.CreateClient();
            var requestData = new
            {
                contents = new[]
                 {
                new
                {
                    parts = new[]
                    {
                        new { text = BuildPromptSpeaking(speakingModel.QuestionName, speakingModel.Answer,speakingModel.PartNumber)

                        }
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
                var scoreMatch = Regex.Match(result, @"\*\*Overall Band Score:\*\*\s*([\d\.]+)");
                if (scoreMatch.Success)
                {
                    var overallScore = scoreMatch.Groups[1].Value;
                    speakingExplains.Score = overallScore;  // Assign the Overall Band Score
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
                speakingExplains.Explain = $"{feedback}\n\nSuggestions for Improvement:\n{suggestions}";
            }

            return speakingExplains;
        }

 
//        private string BuildPromptSpeaking(string questionName, string answer, int part)
//        {
//            return $@"
//### You are an expert in IELTS speaking comprehension. Based on the following context Part {part} Question:
//**Question:** {questionName}

//**User's Response:** 
//{answer}

//Please evaluate the response based on the following criteria and give your answer base on {part} and {questionName}:

//1. **Task Relevance:**
//   - Does the response directly and fully address the question?
//   - How much of the response is off-topic or irrelevant? This is the most important factor when determining the score.

//2. **Task Response:**
//   - Does the response provide clear arguments and relevant examples to support its points?
//   - Are the arguments well-developed and logically presented?

//3. **Coherence and Cohesion:**
//   - Is the response well-structured, with a clear introduction, body, and conclusion (if applicable)?
//   - Are ideas logically organized, and is there a clear flow between ideas and sentences?
//   - Are appropriate transition words and cohesive devices used?

//4. **Lexical Resource:**
//   - Is the vocabulary varied and used accurately?
//   - Are there any errors in word choice, spelling, or repetition? Are more sophisticated words used where possible?

//5. **Grammatical Range and Accuracy:**
//   - Are a variety of sentence structures used effectively?
//   - Are there any significant grammatical errors (e.g., tense errors, incorrect article usage, sentence fragments)?

//### Evaluation Format:
//- **Overall Score:** [Numeric score only, e.g., 3.0, 6.5, 7.5]
//- **Feedback:**
//  - **Task Relevance:** [Provide feedback here]
//  - **Task Response:** [Provide feedback here]
//  - **Coherence and Cohesion:** [Provide feedback here]
//  - **Lexical Resource:** [Provide feedback here]
//  - **Grammatical Range and Accuracy:** [Provide feedback here]
// - **AI Answer:** [Provide answer base on question]
//- **Suggestions for Improvement:** 
//  - [Provide actionable and specific tips for improvement]

//### Important Notes:
//- Task Relevance is the most important factor in determining the score. If the response does not directly address the question, the overall score should be lowered accordingly.
//- Make sure the feedback is constructive and detailed for the user to improve.
//";
//        }




        private string BuildPromptSpeaking(string questionName, string answer, int part)
        {
            return $@"
        ### You are an expert in IELTS Speaking. 
        **Context:** Part {part} of the IELTS Speaking test.

        **Question:** {questionName}

        **User's Response:**
        {answer}


        **Evaluate the response based on the following IELTS Speaking criteria and give your answer :**

        **1 Coherence:**
           
           - **Coherence:** 
              - Is the response well-organized and easy to follow? 
              - Are ideas presented in a logical order with clear connections between them? 
              - Are appropriate discourse markers (e.g., 'firstly,' 'however,' 'in conclusion') used effectively?

        **2. Lexical Resource:**
           - **Vocabulary Range:**
              - Does the speaker use a wide range of vocabulary, including less common and more sophisticated words? 
              - Are there any noticeable repetitions or limited vocabulary?
           - **Accuracy:**
              - Is the vocabulary used accurately and appropriately in context? 
              - Are there any errors in word choice or spelling?

        **3. Grammatical Range and Accuracy:**
           - **Range:**
              - Does the speaker use a variety of grammatical structures, including complex sentences? 
              - Can they effectively use different tenses and verb forms?
           - **Accuracy:**
              - Are there any grammatical errors, such as subject-verb agreement, pronoun use, or article usage? 
              - How significant are the errors and how much do they impact communication?

        **4. Pronunciation:**
           - **Clarity:**
              - Is the speaker's pronunciation clear and easy to understand? 
              - Are individual sounds pronounced accurately?
           - **Fluency:**
              - Does the speaker maintain a natural and fluent rhythm and intonation? 
              - Are there any noticeable problems with stress or intonation that affect communication?

        ### Evaluation Format:

        - **Overall Band Score:** [Numeric score only, e.g., 3.0, 6.5, 7.5,8.5]
        - **Coherence:** [Numeric score only, e.g., 3.0, 6.5, 7.5,8.5]
        - **Lexical Resource:** [Numeric score only, e.g., 3.0, 6.5, 7.5,8.5]
        - **Grammatical Range and Accuracy:** [Numeric score only, e.g., 3.0, 6.5, 7.5,8.5]
        - **Pronunciation:** [Numeric score only, e.g., 3.0, 6.5, 7.5,8.5]

        **Feedback:**
           - **Strengths:** [Highlight positive aspects of the response]
           - **Weaknesses:** [Identify areas for improvement with specific examples]
           - **Suggestions for Improvement:** [Provide actionable and specific tips for improving each criterion]
        **Suggestion Answer** [Provide best answer]
        ### Important Notes:

        - All four criteria are equally important in determining the overall band score.
        - Consider the speaker's performance holistically and how it impacts overall communication. 
        - Provide constructive and specific feedback to help the speaker improve their English.

        ### Scoring Considerations (for AI):

        - **Part 1:** Focus on fluency, coherence, and lexical resource, as this part emphasizes interaction and basic communication.
        - **Part 2:** Prioritize lexical resource, grammatical range and accuracy, and coherence, as this part requires sustained speech and more complex language.
        - **Part 3:** Emphasize lexical resource, grammatical range and accuracy, and fluency and coherence, as this part involves more abstract discussion and deeper analysis.

        - Use a combination of rule-based systems and machine learning models to assess the different aspects of the response.
        - Consider using natural language processing techniques to analyze the text for fluency, coherence, and grammatical accuracy.
        - Implement a scoring rubric that maps specific linguistic features to band scores according to the official IELTS band descriptors.

        This refined prompt provides a more comprehensive and accurate framework for evaluating IELTS Speaking responses. By considering all four criteria and their sub-components, the AI can provide more insightful and helpful feedback to the user.";
        }




        public async Task ExplainListeningAndReading(Guid partId, int skillType)
        
        {
            var sections = await _testExamRepository.GetSectionsByPartId(partId);

            foreach (var section in sections)
            {
                var prompts = new StringBuilder();
                var sectionContext = string.Empty;
                var finalPrompt = "";

                var readingKey = new HashSet<int> { 1, 2, 3, 4, 5, 6 };
                var listeningKey = new HashSet<int> { 6, 8 };

                var isHasSectionContext = (skillType == 0 && !readingKey.Contains(section.SectionType)) ||
                                          (skillType == 1 && !listeningKey.Contains(section.SectionType));


                foreach (var sq in section.SectionQuestions) {

                    var correctAnswers = await _testExamRepository.GetCorrectAnswers(sq.Question.Id, section.SectionType, skillType);
                    if (isHasSectionContext)
                    {
                        foreach (var answer in correctAnswers.Where(a => a?.QuestionId != null))
                        {
                            prompts.AppendLine($"QuestionId: {answer?.QuestionId} and find questionId in sectionContext to know context question");
                            prompts.AppendLine($"Correct Answer: {answer?.AnswerText}");
                            prompts.AppendLine();
                        }
                    }
                    else
                    {
                        if(skillType == 0 && (section.SectionType == 2 || section.SectionType == 3))
                        {
                            prompts.AppendLine($"Question: {correctAnswers[0].AnswerText} - this is a question for True, False, or Not Given.");
                            prompts.AppendLine($"Is it correct?: {(correctAnswers[0].TypeCorrect == 1 ? "Correct" : correctAnswers[0].TypeCorrect == 0 ? "Incorrect" : "Not Given")}");
                        }
                        else
                        {
                            var questionName = await _testExamRepository.GetQuestionNameById(sq.Question.Id);
                            prompts.AppendLine($"Question: {questionName}");
                            prompts.AppendLine($"Correct Answers: {string.Join(", ", correctAnswers.Select(a => a?.AnswerText))}");
                            prompts.AppendLine();
                        }
                    }
                }
             

                if (skillType == 0)
                 {
                    var context = await _testExamRepository.GetContentText(partId);
                    if (isHasSectionContext)
                        finalPrompt = BuildExplainReadingPromptWithSectionContext(prompts.ToString(), context, sectionContext);
                    else
                        finalPrompt = BuildExplainReadingPrompt(prompts.ToString(), context);
                }
                else
                    {
                    string script = string.Empty;
                    var part = await _testExamRepository.GetPartNumber(partId);


                    while (part.AudioProcessingStatus == 0) 
                    {
                        await Task.Delay(2000); 
                        part = await _testExamRepository.GetPartNumber(partId);
                    }

                   
                     script = part.ScriptAudio;
                    if (isHasSectionContext)
                             finalPrompt = BuildExplainListeningPrompt(prompts.ToString(), script);
                    else
                        finalPrompt = BuildExplainLíteningPromptWithSectionContext(prompts.ToString(), script, sectionContext);
                    }


                var requestData = new
                {
                    contents = new[]
                    {
                          new
                          {
                              parts = new[]
                              {
                                  new { text = finalPrompt }
                              }
                          }
                      }
                };

                var client = _clientFactory.CreateClient();
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

                var result = aiResponse?.candidates[0]?.content?.parts[0]?.text?.ToString();

                await _testExamRepository.UpdateExplainSection(section.Id, result);
            }
        }



        private string BuildExplainReadingPrompt(string prompts, string context)
        {
            return $@"
              You are an expert in reading comprehension. Based on the following context and questions, explain why each correct answer is the best choice for each question.
              Context:
              {context}
              Questions and Answers:
              {prompts}
              Instructions:
              For each question listed in the prompts, provide a explanation below the question to justify why the correct answer(s) is the best choice. Maintain clarity and precision in your explanations.
              ";
        }

        private string BuildExplainReadingPromptWithSectionContext(string prompts, string context, string sectionContext)
        {
            return $@"
                  You are an expert in reading comprehension. Based on the following context, the associated questions, and the contextual information provided, offer a detailed explanation for each correct answer. 

                  Context:
                  {context}

                  Section Context:
                  {sectionContext}

                  Questions and Answers:
                  {prompts}

                  Instructions:
                  1. Use the **Section Context** to understand the format of the questions and any specific instructions or requirements (e.g., answer length, word limits, or specific topics).
                  2. For each question listed in the prompts, explain why the correct answer(s) is the best choice. Reference details from:
                     - The context to highlight evidence supporting the answer.
                     - The section context to ensure the answer adheres to the requirements.
                  3. Your explanation should be clear, precise, and directly address how the answer is supported by the provided information. 
                  4. Where applicable, describe why other potential answers are incorrect.

                  Provide thorough and structured explanations that align with the expectations of a reading comprehension expert.
                  ";
        }

        private string BuildExplainListeningPrompt(string prompts, string transcriptAudio)
        {
            return $@"
        You are an expert in listening comprehension. Based on the following audio transcript and the associated questions, provide a detailed explanation for each correct answer. 

        Audio Transcript:
        {transcriptAudio}

        Questions and Answers:
        {prompts}

        Instructions:
        For each question listed in the prompts, explain why the correct answer(s) is the best choice. Your explanation should reference specific details from the transcript and offer precise reasoning. Maintain clarity and accuracy in your responses, and ensure that the explanation directly addresses why each correct answer is supported by the audio content.
        ";
        }

        private string BuildExplainLíteningPromptWithSectionContext(string prompts, string transcriptAudio, string sectionContext)
        {
                        return $@"
            You are an expert in listening comprehension. Based on the following audio transcript, the associated questions, and the contextual information provided, offer a detailed explanation for each correct answer. 

            Audio Transcript:
            {transcriptAudio}

            Section Context:
            {sectionContext}

            Questions and Answers:
            {prompts}

            Instructions:
            1. Use the **Section Context** to understand the format of the questions and any specific instructions or requirements (e.g., answer length, word limits, or specific topics).
            2. For each question listed in the prompts, explain why the correct answer(s) is the best choice. Reference details from:
               - The audio transcript to highlight evidence supporting the answer.
               - The section context to ensure the answer adheres to the requirements.
            3. Your explanation should be clear, precise, and directly address how the answer is supported by the provided information. 
            4. Where applicable, describe why other potential answers are incorrect.

            Provide thorough and structured explanations that align with the expectations of a listening comprehension expert.
            ";
                    }



    }
}
