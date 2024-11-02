using AutoMapper;
using Entity.Test;
using IRepository;
using IService;
using Microsoft.AspNetCore.Mvc;
using Model.Test;
using System.Security.Claims;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Newtonsoft.Json; // For .xlsx files
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



       
        [HttpPost("")]
        public async Task<IActionResult> CreateTest([FromBody] TestModel model)
        {

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userRoleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            var result = await _testExamService.CreateTestAsync(userId, model, userRoleClaim);
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IEnumerable<TestModel>> GetAllTestsAsync([FromRoute] Guid userId)
        {

            var tests = await _testRepository.GetAllTestsAsync(userId); 
            return _mapper.Map<IEnumerable<TestModel>>(tests);
        }

        [HttpGet("{id}/testDetail")]
        public async Task<TestModel> GetTestAsync([FromRoute] Guid id)
        {
            var test = await _testRepository.GetTestAsync(id);
            return _mapper.Map<TestModel>(test);
        }


        [HttpGet("{sectionCourseId}/sectionCourseId")]
        public async Task<TestModel> GetTestBySecionCourseId([FromRoute] Guid sectionCourseId)
        {
            var test = await _testRepository.GetTestBySecionCourseId(sectionCourseId);
            return _mapper.Map<TestModel>(test);
        }
        


        [HttpGet("{id}/skills")]
        public async Task<List<Skill>> GetSkills([FromRoute] Guid id)
        {
            var skills = await _testRepository.GetSkills(id);
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

            // Group the response data by skill type
            var responseData = new Dictionary<string, object>();

            string skillTypeKey = skill.Type switch
            {
                0 => "reading",
                1 => "listening",
                2 => "writing",
                3 => "speaking",
                _ => "unknown"
            };

            responseData[skillTypeKey] = new
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
                    questionName = $"Part {part.PartNumber}",
                    sections = part.Sections.Select(section => new
                    {
                        id = section.Id,
                        sectionGuide = section.SectionGuide,
                        sectionType = section.SectionType,
                        image = section.Image,
                        questions = section.SectionQuestions.Select(sq => new
                        {
                            id = sq.Question.Id,
                            questionName = sq.Question.QuestionName,
                            answers = sq.Question.Answers?.Select(ans => new
                            {
                                id = ans.Id,
                                answerText = ans.AnswerText,
                                isCorrect = ans.TypeCorrect
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

                // Add skill data to the response dictionary under its skill type
                responseData[skillTypeKey] = new
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
                        questionName = $"Part {part.PartNumber}",
                        sections = part.Sections.Select(section => new
                        {
                            id = section.Id,
                            sectionGuide = section.SectionGuide,
                            sectionType = section.SectionType,
                            image = section.Image,
                            questions = section.SectionQuestions.Select(sq => new
                            {
                                id = sq.Question.Id,
                                questionName = sq.Question.QuestionName,
                                answers = sq.Question.Answers?.Select(ans => new
                                {
                                    id = ans.Id,
                                    answerText = ans.AnswerText,
                                    isCorrect = ans.TypeCorrect
                                }).ToList()
                            }).ToList()
                        }).ToList()
                    }).ToList()
                };
            }

            return Ok(responseData);
        }


        [HttpGet("{id}/parts")]
        public async Task<List<Part>> GetParts([FromRoute] Guid id)
        {
            var parts = await _testRepository.GetParts(id);
            return parts;
        }


        [HttpPost("questionsBank/import")]
        public async Task<IActionResult> ImportQuestions([FromForm] IFormFile file)
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
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0; // Reset stream position for reading

                    IWorkbook workbook = new XSSFWorkbook(stream);
                    ISheet worksheet = workbook.GetSheetAt(0); // Get the first worksheet

                    for (int row = 1; row <= worksheet.LastRowNum; row++) // Start from the second row
                    {
                        var rowData = worksheet.GetRow(row);
                        if (rowData == null) continue; // Skip empty rows

                        var question = new Question
                        {
                            QuestionName = rowData.GetCell(0)?.ToString(),
                            QuestionType = int.TryParse(rowData.GetCell(1)?.ToString(), out var qType) ? qType : 0,
                            Skill = int.TryParse(rowData.GetCell(2)?.ToString(), out var skill) ? skill : 0,
                            PartNumber = int.TryParse(rowData.GetCell(3)?.ToString(), out var part) ? part : 0,
                            Answers = new List<Answer>()
                        };

                        var answerCell = rowData.GetCell(4)?.ToString();
                        if (!string.IsNullOrWhiteSpace(answerCell))
                        {
                            // Detect if the answer is JSON or plain text
                            if (answerCell.TrimStart().StartsWith("{"))
                            {
                                // Attempt to parse JSON format
                                try
                                {
                                    var answerObj = JsonConvert.DeserializeObject<Answer>(answerCell);
                                    if (answerObj != null)
                                    {
                                        question.Answers.Add(answerObj);
                                    }
                                }
                                catch (JsonException jsonEx)
                                {
                                    return BadRequest($"JSON parsing error in answer cell: {jsonEx.Message}");
                                }
                            }
                            else
                            {
                                // Plain text answer, default TypeCorrect to 0
                                question.Answers.Add(new Answer
                                {
                                    AnswerText = answerCell,
                                    TypeCorrect = 0 // Default value for plain text answer
                                });
                            }
                        }

                        questions.Add(question);
                    }
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


        [HttpGet("questionsBank/{userId}")]
        public async Task<IActionResult> GetQuestionsAsync([FromRoute] Guid userId)
        {
            try
            {
                var questions = await _testRepository.GetAllQuestionsAsync(userId);

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
        [HttpPost("questionsBank")]
        public async Task<IActionResult> CreateQuestionsAsync([FromBody] List<QuestionDto> questionModels)
        {
            if (questionModels == null || !questionModels.Any())
            {
                return BadRequest("No questions provided.");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid user ID.");
            }

            try
            {
                // Map QuestionModel to Question
                var questions = _mapper.Map<List<Question>>(questionModels);

                // Set UserId for each question
                foreach (var question in questions)
                {
                    question.UserId = userId; // Assuming the UserId is part of the Question entity
                }

                // Call the repository method to add questions
                await _testRepository.AddQuestionsAsync(questions);
                return Ok("Questions created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("questionsBank/{id}")]
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

        [HttpPut("questionsBank/{id}")]
        public async Task<IActionResult> UpdateQuestionAsync([FromRoute] Guid id, [FromBody] QuestionResponse updatedQuestion)
        {
            if (updatedQuestion == null)
            {
                return BadRequest("Question data is null.");
            }

            // Check if the question exists
            var existingQuestion = await _testRepository.GetQuestionByIdAsync(id);
            if (existingQuestion == null)
            {
                return NotFound($"Question with ID {id} not found.");
            }

            // Ensure the updated question ID matches the route ID
            if (updatedQuestion.Id != id)
            {
                return BadRequest("Question ID mismatch.");
            }

            try
            {
                await _testRepository.UpdateQuestionAsync(updatedQuestion);
                return Ok($"Question with ID {id} updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
