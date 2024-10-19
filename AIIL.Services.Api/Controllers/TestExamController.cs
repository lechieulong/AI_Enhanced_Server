using AutoMapper;
using Entity.Test;
using IRepository;
using IService;
using Microsoft.AspNetCore.Mvc;
using Model.Test;
using System.Security.Claims;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // For .xlsx files
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

        [HttpPost("")]
        public async Task<IActionResult> CreateTest([FromBody] TestModel model)
        {
            var result = await _testExamService.CreateTestAsync(model);
            return Ok(result);
        }

        [HttpGet("")]
        public async Task<IEnumerable<TestModel>> GetAllTestsAsync([FromRoute] Guid userId)
        {
            var tests = await _testRepository.GetAllTestsAsync(userId); 
            return _mapper.Map<IEnumerable<TestModel>>(tests);
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

                    // Create an IWorkbook instance from the stream
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

                        // Assuming that answers are in the same row for simplicity
                        var answerText = rowData.GetCell(4)?.ToString();
                        var isCorrect = int.TryParse(rowData.GetCell(5)?.ToString(), out var correct) ? correct : 0;

                        if (!string.IsNullOrWhiteSpace(answerText))
                        {
                            question.Answers.Add(new Answer
                            {
                                AnswerText = answerText,
                                IsCorrect = isCorrect
                            });
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
        public async Task<IActionResult> CreateQuestionsAsync([FromBody] List<QuestionModel> questionModels)
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
