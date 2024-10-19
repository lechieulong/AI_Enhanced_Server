using AIIL.Services.Api.Extensions; // Adjust the namespace as needed
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IService; // Adjust the namespace as needed
using System.IO;
using System.Threading.Tasks;
using System.Security.Claims;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/demo")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly IBlogStorageService _blobStorageService;

        public DemoController(IBlogStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        [HttpPost("upload")]
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
    }
}
