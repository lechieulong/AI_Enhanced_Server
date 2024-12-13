using AIIL.Services.Api.Extensions; // Adjust the namespace as needed
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IService; // Adjust the namespace as needed
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;
using Service;
using Entity.Test;
using Model.Test;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly IBlogStorageService _blobStorageService;
        private readonly IAzureService _azureService;
        private readonly IGeminiService _geminiService;



        public CommonController(IBlogStorageService blobStorageService, IAzureService azureService, IGeminiService geminiService)
        {
            _blobStorageService = blobStorageService;
            _azureService = azureService;
            _geminiService = geminiService;
        }

        


        [HttpPost("scoreSpeaking")]
        public async Task<IActionResult> ScoreSpeaking([FromBody] SpeakingModel model)
        {

            try
            {

                var result = await _geminiService.ScoreSpeaking(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing image: {ex.Message}");
            }
        }


        [HttpPost("extract-text")]
        public async Task<IActionResult> ExtractTextFromImage([FromBody]string imageFile)
        {

            try
            {
               
                    var result = await _azureService.ExtractTextFromImageAsync(imageFile);
                    return Ok(new { extractedText = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing image: {ex.Message}");
            }
        }


    
            [HttpPost("transcribe")]
            public async Task<IActionResult> TranscribeAudio([FromBody] string fileUrl)
            {
                try
                {
                    var transcription = await _azureService.ProcessAndTranscribeAudioFromBlobAsync(fileUrl);
                    return Ok(new { Success = true, Transcription = transcription });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Success = false, Message = ex.Message });
                }
            }

        [HttpPost("upload")]
        [RequestSizeLimit(100_000_000)] // Allow up to 100 MB
        [RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)] // Multipart form limits
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return BadRequest("User ID is required.");

            using (var stream = file.OpenReadStream())
            {
                var fileUrl = await _blobStorageService.UploadFileAsync(userId, stream, file.FileName, file.ContentType);
                return Ok(new { FileUrl = fileUrl });
            }
        }

        [HttpGet("template")]
        public async Task<IActionResult> GetDownloadTemplate()
        {
            try
            {
                var fileUrl = await _blobStorageService.DownloadTemplate();
                return Ok(new { FileUrl = fileUrl });
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("upload-course-file")]
        public async Task<IActionResult> UploadFileForCourse([FromQuery] string type, [FromQuery] string id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(id))
                return BadRequest("Type and ID are required.");

            string containerName;
            string path;
            bool useCourseStorage = false; 

            if (type.Equals("courseLesson", StringComparison.OrdinalIgnoreCase))
            {
                containerName = "courselessoncontent";
                path = $"/{id}";
                useCourseStorage = true; 
            }
            else if (type.Equals("class", StringComparison.OrdinalIgnoreCase))
            {
                containerName = "class";
                path = $"/{id}";
                useCourseStorage = true;
            }
            else if (type.Equals("course", StringComparison.OrdinalIgnoreCase))
            {
                containerName = "course";
                path = $"/{id}";
                useCourseStorage = true; 
            }
            else
            {
                return BadRequest("Invalid type. Valid types are 'courseLesson', 'class', or 'course'.");
            }

            using (var stream = file.OpenReadStream())
            {
                try
                {
                    var fileUrl = await _blobStorageService.UploadFileCourseAsync(
                        containerName,
                        path,
                        stream,
                        file.FileName,
                        file.ContentType,
                        useCourseStorage
                    );

                    return Ok(new { FileUrl = fileUrl });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error uploading file: {ex.Message}");
                }
            }
        }


    }
}
