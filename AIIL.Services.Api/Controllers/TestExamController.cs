using AutoMapper;
using Entity.Test;
using IRepository;
using IService;
using Microsoft.AspNetCore.Mvc;
using Model.Test;
using System.Security.Claims;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json;
using Service;
using Model.Utility;
using Microsoft.AspNetCore.Authorization;
using Model; // For .xlsx files
namespace AIIL.Services.Api.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestExamController : ControllerBase
    {
        private readonly ITestExamService _testExamService;
        private readonly ITestExamRepository _testRepository;
        private readonly IMapper _mapper;

        public TestExamController(ITestExamService testExamService, ITestExamRepository testRepository, IMapper mapper)
        {
            _testExamService = testExamService;
            _testRepository = testRepository;
            _mapper = mapper;
        }

        [HttpGet("tests")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetTests(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and page size must be greater than zero.");
            }

            var (tests, totalCount) = await _testRepository.GetTestsAsync(page, pageSize);

            // Ensure the total count is always non-negative
            totalCount = totalCount < 0 ? 0 : totalCount;

            var response = new
            {
                Tests = tests,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(response);
        }

        [HttpPost("skills/{testId}")]
        public async Task<IActionResult> CreateSkills([FromRoute] Guid testId, [FromBody] SkillDtos skillsDto)
        {


            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            if (skillsDto == null || skillsDto.Skills == null)
            {
                return BadRequest("The skills data cannot be null.");
            }

            try
            {
                await _testRepository.CreateSkillsAsync(userId, testId, skillsDto.Skills);
                return Ok("Skills created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("update-test")]
        [Authorize(Roles = "ADMIN,TEACHER")]
        public async Task<IActionResult> UpdateTest([FromBody] TestUpdateDto testModel)
        {
            if (testModel == null)
            {
                return BadRequest("Test model is null.");
            }

            try
            {
                var testExamEntity = _mapper.Map<TestExam>(testModel);

                var updatedTest = await _testRepository.UpdateTestAsync(testExamEntity);

                if (updatedTest == null)
                {
                    return NotFound("Test not found.");
                }

                return Ok(updatedTest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "ADMIN,TEACHER")]
        public async Task<IActionResult> DeleteTest(Guid id)
        {
            try
            {
                bool isDeleted = await _testRepository.DeleteTestAsync(id);
                if (!isDeleted)
                {
                    return BadRequest("Failed to delete test.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
            return Ok("Test deleted successfully.");
        }

        [HttpPost("{testId}/submitTest/{userId}")]
        public async Task<IActionResult> CalculateScore([FromRoute] Guid testId, [FromRoute] Guid userId, [FromBody] SubmitTestDto model)
        {
            var result = await _testExamService.CalculateScore(testId, userId, model);
            //var cc = 0
            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateTest([FromBody] TestModel model)
        {

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            // Lấy tất cả các claims liên quan đến vai trò
            var userRoleClaims = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            int role = userRoleClaims.Contains(SD.Admin) ? 1 : 0;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var result = await _testExamService.CreateTestAsync(userId, model, role);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IEnumerable<TestModel>> GetAllTestsAsync([FromRoute] Guid userId)
        {

            var tests = await _testRepository.GetAllTestsAsync(userId);
            return _mapper.Map<IEnumerable<TestModel>>(tests);
        }

        [HttpGet("admintests")]
        public async Task<IActionResult> GetAdminTests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 6)
        {
            // Validate parameters
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page number and size must be greater than 0");

            var pagedTests = await _testExamService.GetPagedAdminTests(pageNumber, pageSize);

            // Include total count for pagination metadata
            var totalCount = await _testRepository.GetTotalAdminTestsCount();
            var response = new
            {
                Data = pagedTests,
                TotalCount = pagedTests.Count() == 0 ? 0 : totalCount,
                TotalPages = pagedTests.Count() == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(response);
        }


        [HttpGet("{id}/testDetail")]
        public async Task<TestModel> GetTestAsync([FromRoute] Guid id)
        {
            var test = await _testRepository.GetTestAsync(id);
            return _mapper.Map<TestModel>(test);
        }

        [HttpPost("{userId}/result")]
        public async Task<IActionResult> GetResultTest([FromRoute]Guid  userId, [FromBody] ResultPayloadDto request)
        {

            var guids = request.SkillResultIds
                      .Select(skillResult => skillResult.Id)
                      .ToList();

            var test = await _testRepository.GetResultTest( userId, guids);
            return Ok(test);
        }


        [HttpGet("{sectionCourseId}/sectionCourseId")]
        public async Task<TestModel> GetTestBySecionCourseId([FromRoute] Guid sectionCourseId)
        {
            var test = await _testRepository.GetTestBySecionCourseId(sectionCourseId);
            return _mapper.Map<TestModel>(test);
        }

        [HttpGet("{skillId}/skillType")]
        public async Task<IActionResult> GetSkillById([FromRoute] Guid skillId)
        {
            var skill = await _testRepository.GetSkillByIdNe(skillId);
            return Ok(skill);
        }


        [HttpGet("{testId}/skills")]
        public async Task<List<Skill>> GetSkills([FromRoute] Guid testId)
        {
            var skills = await _testRepository.GetSkills(testId);
            return skills;
        }

        [HttpGet("{skillId}/skill")]
        public async Task<IActionResult> GetSkillData(Guid skillId)
        {
            var skill = await _testRepository.GetSkillByIdAsync(skillId);
            if (skill == null)
            {
                return NotFound("Skill not found");
            }

            var responseData = new Dictionary<string, object>();

            string skillTypeKey = skill.Type switch
            {
                0 => "reading",
                1 => "listening",
                2 => "writing",
                3 => "speaking",
                _ => "unknown"
            };
            var skillType = skill.Type;
            responseData[skillTypeKey] = new
            {
                id = skill.Id,
                duration = skill.Duration,
                type = skill.Type,
                parts = skill.Parts.OrderBy(part => part.PartNumber) // Sắp xếp tăng dần theo PartNumber
                    .Select(part => new
                    {
                    id = part.Id,
                    partNumber = part.PartNumber,
                    contentText = part.ContentText,
                    audio = part.Audio,
                    image = part.Image,
                    questionName = $"Part {part.PartNumber}",
                    sections = part.Sections.OrderBy(s => s.SectionOrder).Select(section => new
                    {
                        id = section.Id,
                        sectionGuide = section.SectionGuide,
                        sectionType = section.SectionType,
                        image = section.Image,
                        sectionContext = section.SectionContext,
                        questions = section.SectionQuestions.OrderBy(sq => sq.QuestionOrder) 
                            .Select(sq => new
                            {
                                id = sq.Question.Id,
                                questionName = sq.Question.QuestionName,
                                questionOrder = sq.QuestionOrder,
                                questionType = sq.Question.QuestionType,
                                answers = sq.Question.Answers?.Select(ans => new
                                {
                                    id = ans.Id,
                                    answerText = (skillType == 0 && new[] { 1, 2, 3, 4, 5, 6 }.Contains(section.SectionType)) ||
                                    (skillType == 1 && new[] { 5, 6, 8 }.Contains(section.SectionType)) ? ans.AnswerText : "",
                                }).ToList()
                            }).ToList()
                    }).ToList()
                }).ToList()
            };

            return Ok(responseData);
        }

        [HttpGet("{testId}/testing")]
        public async Task<IActionResult> GetTesting(Guid testId)
        {
            var skills = await _testRepository.GetSkillsByTestIdAsync(testId);

            if (skills == null || !skills.Any())
            {
                return NotFound("No skills found for the given testId.");
            }

            var sortedSkills = skills.OrderBy(skill => skill.Type).ToList();
            var responseData = new Dictionary<string, object>();

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
                var skillType = skill.Type;
                responseData[skillTypeKey] = new
                {
                    id = skill.Id,
                    duration = skill.Duration,
                    type = skill.Type,
                    parts = skill.Parts.OrderBy(part => part.PartNumber) // Sắp xếp tăng dần theo PartNumber
                    .Select(part => new
                    {
                        id = part.Id,
                        partNumber = part.PartNumber,
                        contentText = part.ContentText,
                        audio = part.Audio,
                        image = part.Image,
                        sections = part.Sections.OrderBy(s => s.SectionOrder).Select(section => new
                        {
                            id = section.Id,
                            sectionGuide = section.SectionGuide,
                            sectionType = section.SectionType,
                            image = section.Image,
                            sectionContext = section.SectionContext,
                            questions = section.SectionQuestions.OrderBy(sq => sq.QuestionOrder)
                            .Select(sq => new
                            {
                                id = sq.Question.Id,
                                questionName = sq.Question.QuestionName,
                                questionOrder = sq.QuestionOrder,
                                questionType = sq.Question.QuestionType,
                                answers = sq.Question.Answers?.Select(ans => new
                                {
                                    id = ans.Id,
                                    answerText = (skillType == 0 && new[] { 1, 2, 3, 4, 5, 6 }.Contains(section.SectionType)) ||
                                    (skillType == 1 && new[] {5,6,8}.Contains(section.SectionType)) ? ans.AnswerText : "",
                                }).ToList()
                            }).ToList()
                        }).ToList()
                    }).ToList()
                };
            }

            return Ok(responseData);
        }


        [HttpPost("testExplain")]
        public async Task<IActionResult> GetExplainTest(TestExplainRequestDto model) {
            if (model.TestId == Guid.Empty) return BadRequest("TestId is empty");

            var result = await _testExamService.GetExplainByTestId(model);
            return Ok(result);
        }

        [HttpGet("{id}/parts")]
        public async Task<List<Part>> GetParts([FromRoute] Guid id)
        {
            var parts = await _testRepository.GetParts(id);
            return parts;
        }


        //[HttpPost("questionsBank/import")]
        //public async Task<IActionResult> ImportQuestions([FromForm] IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("No file uploaded.");
        //    }

        //    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        //    {
        //        return Unauthorized("Invalid user ID.");
        //    }

        //    var questions = new List<Question>();

        //    try
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);
        //            stream.Position = 0; // Reset stream position for reading

        //            IWorkbook workbook = new XSSFWorkbook(stream);
        //            ISheet worksheet = workbook.GetSheetAt(0); // Get the first worksheet

        //            // Debugging log for row detection
        //            Console.WriteLine($"Total rows in worksheet (LastRowNum): {worksheet.LastRowNum}");

        //            for (int row = 1; row <= worksheet.LastRowNum; row++) // Start from the second row
        //            {
        //                var rowData = worksheet.GetRow(row);

        //                // Check if the row is null or all cells are empty
        //                if (rowData == null || rowData.Cells.All(cell => cell == null || string.IsNullOrWhiteSpace(cell.ToString())))
        //                {
        //                    Console.WriteLine($"Skipping empty row: {row}");
        //                    continue;
        //                }

        //                var question = new Question
        //                {
        //                    QuestionName = rowData.GetCell(0)?.ToString(),
        //                    QuestionType = int.TryParse(rowData.GetCell(1)?.ToString(), out var qType) ? qType : 0,
        //                    Skill = int.TryParse(rowData.GetCell(2)?.ToString(), out var skill) ? skill : 0,
        //                    Explain = "",
        //                    PartNumber = int.TryParse(rowData.GetCell(3)?.ToString(), out var part) ? part : 0,
        //                    Answers = new List<Answer>()
        //                };

        //                var answerCell = rowData.GetCell(4)?.ToString();
        //                if (!string.IsNullOrWhiteSpace(answerCell))
        //                {
        //                    // Detect if the answer is JSON or plain text
        //                    if (answerCell.TrimStart().StartsWith("[") && answerCell.TrimEnd().EndsWith("]"))
        //                    {
        //                        // Attempt to parse JSON array format
        //                        try
        //                        {
        //                            var answers = JsonConvert.DeserializeObject<List<Answer>>(answerCell);
        //                            if (answers != null && answers.Any())
        //                            {
        //                                foreach (var answer in answers)
        //                                {
        //                                    // Validate individual answer
        //                                    if (!string.IsNullOrWhiteSpace(answer.AnswerText))
        //                                    {
        //                                        question.Answers.Add(new Answer
        //                                        {
        //                                            Id = Guid.NewGuid(),
        //                                            AnswerText = answer.AnswerText,
        //                                            TypeCorrect = answer.TypeCorrect
        //                                        });
        //                                    }
        //                                }
        //                            }
        //                        }
        //                        catch (JsonException jsonEx)
        //                        {
        //                            return BadRequest($"JSON parsing error in answer cell: {jsonEx.Message}");
        //                        }
        //                    }
        //                    else if (answerCell.TrimStart().StartsWith("{"))
        //                    {
        //                        // Attempt to parse single JSON object format
        //                        try
        //                        {
        //                            var answerObj = JsonConvert.DeserializeObject<Answer>(answerCell);
        //                            if (answerObj != null && !string.IsNullOrWhiteSpace(answerObj.AnswerText))
        //                            {
        //                                question.Answers.Add(new Answer
        //                                {
        //                                    Id = Guid.NewGuid(),
        //                                    AnswerText = answerObj.AnswerText,
        //                                    TypeCorrect = answerObj.TypeCorrect
        //                                });
        //                            }
        //                        }
        //                        catch (JsonException jsonEx)
        //                        {
        //                            return BadRequest($"JSON parsing error in answer cell: {jsonEx.Message}");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        // Plain text answer, default TypeCorrect to 0
        //                        question.Answers.Add(new Answer
        //                        {
        //                            Id = Guid.NewGuid(),
        //                            AnswerText = answerCell,
        //                            TypeCorrect = 0 // Default value for plain text answer
        //                        });
        //                    }
        //                }


        //                questions.Add(question);
        //            }
        //        }

        //        // Call the service method to import questions
        //        await _testRepository.ImportQuestionAsync(questions, userId);
        //        return Ok("Questions imported successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}



        [HttpPost("questionsBank/import")]
        public async Task<IActionResult> ImportQuestionsFromTxt([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var questions = new List<Question>();

            try
            {
                using (var stream = new StreamReader(file.OpenReadStream()))
                {
                    string line;
                    Question currentQuestion = null;

                    while ((line = await stream.ReadLineAsync()) != null)
                    {
                        line = line.Trim();
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        if (line.StartsWith("questionName:"))
                        {
                            if (currentQuestion != null) // Save the previous question if one exists
                            {
                                questions.Add(currentQuestion);
                            }

                            // Create new question object
                            currentQuestion = new Question();
                            currentQuestion.Answers = new List<Answer>();
                            currentQuestion.QuestionName = line.Replace("questionName:", "").Trim();
                        }
                        else if (line.StartsWith("questionType:"))
                        {
                            var questionTypeString = line.Replace("questionType:", "").Trim();
                            currentQuestion.QuestionType = questionTypeString switch
                            {
                                "MultipleChoice" => 1,
                                "SingleChoice" => 2,
                                "TrueFalse" => 3,
                                "NotGiven" => 4,
                                _ => 0
                            };
                        }
                        else if (line.StartsWith("skill:"))
                        {
                            string skill = line.Replace("skill:", "").Trim();
                            currentQuestion.Skill = skill switch
                            {
                                "reading" => 0,
                                "listening" => 1,
                                "writing" => 2,
                                "speaking" => 3,
                                _ => 0
                            };
                        }
                        else if (line.StartsWith("part:"))
                        {
                            currentQuestion.PartNumber = int.TryParse(line.Replace("part:", "").Trim(), out int part) ? part : 0;
                        }
                        else if (line.StartsWith("answers:"))
                        {
                            var answerString = line.Replace("answers:", "").Trim();

                            if (string.IsNullOrEmpty(answerString) || answerString == "[]")
                            {
                                return BadRequest("Answers are required for each question.");
                            }

                            // Validate and parse answers (JSON array)
                            try
                            {
                                var answers = JsonConvert.DeserializeObject<List<Answer>>(answerString);
                                foreach (var answer in answers)
                                {
                                    if (string.IsNullOrWhiteSpace(answer.AnswerText))
                                    {
                                        return BadRequest("Each answer must have non-empty text.");
                                    }
                                    currentQuestion.Answers.Add(new Answer
                                    {
                                        Id = Guid.NewGuid(),
                                        AnswerText = answer.AnswerText,
                                        TypeCorrect = answer.TypeCorrect
                                    });
                                }
                            }
                            catch (JsonException ex)
                            {
                                return BadRequest($"Error parsing answers JSON: {ex.Message}");
                            }
                        }
                    }

                    // Add the last question if there was one
                    if (currentQuestion != null)
                    {
                        // Validation for skill and part ranges based on skill
                        var isValid = ValidateSkillAndPart(currentQuestion);
                        if (!isValid)
                        {
                            return BadRequest("Invalid part number or question type for the given skill.");
                        }

                        // Additional validation for question types per skill
                        var isValidType = ValidateQuestionTypeForSkill(currentQuestion);
                        if (!isValidType)
                        {
                            return BadRequest("Invalid question type for the given skill.");
                        }

                        questions.Add(currentQuestion);
                    }
                }

                // Ensure that we have questions to import
                if (!questions.Any())
                {
                    return BadRequest("No valid questions found in the file.");
                }

                // Call the service method to import questions
                await _testRepository.ImportQuestionAsync(questions, userId);
                return Ok("Questions imported successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool ValidateSkillAndPart(Question question)
        {
            switch (question.Skill)
            {
                case 0: // Reading
                    if (question.PartNumber < 1 || question.PartNumber > 3)
                    {
                        return false; // Reading only allows part 1, 2, or 3
                    }
                    break;
                case 1: // Listening
                    if (question.PartNumber < 1 || question.PartNumber > 4)
                    {
                        return false; // Listening allows part 1, 2, 3, or 4
                    }
                    break;
                case 2: // Writing
                    if (question.PartNumber < 1 || question.PartNumber > 2)
                    {
                        return false; // Writing only allows part 1 or 2
                    }
                    question.QuestionType = 0; // Writing type should default to 0
                    break;
                case 3: // Speaking
                    if (question.PartNumber < 1 || question.PartNumber > 3)
                    {
                        return false; // Speaking only allows part 1, 2, or 3
                    }
                    question.QuestionType = 0; // Speaking type should default to 0
                    break;
                default:
                    return false; // Invalid skill
            }

            return true;
        }

        private bool ValidateQuestionTypeForSkill(Question question)
        {
            switch (question.Skill)
            {
                case 0: // Reading
                    if (question.QuestionType != 1 && question.QuestionType != 3 && question.QuestionType != 4)
                    {
                        return false; // Reading only allows MultipleChoice, TrueFalse, or NotGiven types
                    }
                    break;
                case 1: // Listening
                    if (question.QuestionType == 1) // MultipleChoice is allowed for Listening, but it must be 8
                    {
                        if (question.QuestionType != 8)
                        {
                            return false; // Listening allows MultipleChoice, but value must be 8
                        }
                    }
                    else if (question.QuestionType != 2) // SingleChoice is allowed for Listening
                    {
                        return false;
                    }
                    break;
                case 2: // Writing
                case 3: // Speaking
                        // Writing and Speaking cannot have MultipleChoice, SingleChoice, TrueFalse, or NotGiven
                    if (question.QuestionType != 0)
                    {
                        return false;
                    }
                    break;
                default:
                    return false; // Invalid skill
            }

            return true;
        }


        [HttpGet("{sectionType}/questionsBank/{userId}/skill/{skill}")]
        public async Task<IActionResult> GetQuestionsAsync(
             [FromRoute] Guid userId,
             [FromRoute] int sectionType,
             [FromRoute] int skill,
             [FromQuery] int page ,  // Default page is 1
             [FromQuery] int pageSize) // Default pageSize is 10
        {
            try
            {
                var questions = await _testRepository.GetQuestionsBySecionTypeAsync(userId, skill,sectionType, page, pageSize);

                if (questions == null || !questions.Any())
                {
                    return NotFound("No questions found for the specified user.");
                }

                return Ok(_mapper.Map<List<QuestionResponse>>(questions));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("questionsBank/{userId}")]
        public async Task<IActionResult> GetQuestionsAsync(
             [FromRoute] Guid userId,
             [FromQuery] int page,  // Default page is 1
             [FromQuery] int pageSize) // Default pageSize is 10
        {
            try
            {
                var questions = await _testRepository.GetQuestionsAsync(userId,  page, pageSize);

                if (questions == null || !questions.Any())
                {
                    return NotFound("No questions found for the specified user.");
                }

                return Ok(_mapper.Map<List<QuestionResponse>>(questions));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("testSubmitted/{userId}")]
        public async Task<IActionResult> GetTestSubmitted(
          [FromRoute] Guid userId,
          [FromQuery] int page,  // Default page is 1
          [FromQuery] int pageSize) // Default pageSize is 10
        {
            try
            {
                var tests = await _testRepository.GetTestSubmittedAsync(userId, page, pageSize);

                return Ok(tests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("testAnalysis/{userId}")]
        public async Task<IActionResult> GetTestAnalysisAttempt([FromRoute] Guid userId)
        {
            try
            {
                var tests = await _testRepository.GetTestAnalysisAttempt(userId);

                return Ok(tests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("attempts/{userId}")]
        public async Task<IActionResult> GetAttemptTests([FromRoute] Guid userId)
        {
            try
            {
                var tests = await _testRepository.GetAttemptTests(userId);

                return Ok(tests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpPost("questionsBank")]
        public async Task<IActionResult> CreateQuestionsAsync([FromBody] QuestionBankModel model)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            try
            {
                var question = new Question
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    QuestionName = model.QuestionName,
                    PartNumber = model.Part,
                    Explain = string.Empty, // Set default if needed
                    Skill = model.SkillType,
                    QuestionType = model.QuestionType // Ensure this is added if needed
                };

                // Initialize Answers collection if not initialized
                question.Answers = new List<Answer>();

                foreach (var answerDto in model.Answers)
                {
                    var answer = new Answer
                    {
                        Id = Guid.NewGuid(),
                        AnswerText = answerDto.AnswerText,
                        TypeCorrect = (int)answerDto.IsCorrect, // Assuming IsCorrect is an int
                        Question = question // EF will automatically set QuestionId
                    };

                    // Adding answer to question's answers collection
                    question.Answers.Add(answer);
                }

                // Add question and its associated answers to the context
                await _testRepository.AddQuestionsAsync(question);

                return Ok("Questions created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpDelete("questionsBank/{id}/delete")]
        public async Task<IActionResult> DeleteQuestionAsync(Guid id)
        {
            // Check if the question exists
            var question = await _testRepository.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound($"Question with ID {id} not found.");
            }

            try
            {
                // Call the repository method to delete the question
                await _testRepository.DeleteQuestionAsync(id);
                return Ok($"Question with ID {id} deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("questionsBank/{id}/update")]
        public async Task<IActionResult> UpdateQuestionAsync([FromRoute] Guid id, [FromBody] QuestionBankModel model)
        {
            if (model == null)
            {
                return BadRequest("Question data is null.");
            }
            try
            {
                await _testRepository.UpdateQuestionAsync(id,model);

                return Ok($"Question with ID {id} updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("test-by-course/{courseId}")]
        public async Task<IActionResult> GetTestsByCourseId(Guid courseId, int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and pageSize must be greater than zero.");
            }

            try
            {
                var (tests, totalCount) = await _testRepository.GetTestExamByCourseIdAsync(courseId, page, pageSize);

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return Ok(new
                {
                    Data = tests,
                    Pagination = new
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = totalCount,
                        TotalPages = totalPages
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("test-history/{courseId}")]
        [Authorize]
        public async Task<IActionResult> GetTestResultHistory(Guid courseId)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    return BadRequest("User not found.");
                }

                var (testResults, totalCount) = await _testExamService.GetTestResultByUserIdAsync(courseId, userIdClaim);

                return Ok(new
                {
                    Data = testResults,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("test-results/{testId}")]
        //[Authorize]
        public async Task<IActionResult> GetTestResultOfATest(Guid testId)
        {
            try
            {
                var (testResults, totalCount) = await _testRepository.GetTestResultOfTest(testId);

                return Ok(new
                {
                    Data = testResults,
                    TotalCount = totalCount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
