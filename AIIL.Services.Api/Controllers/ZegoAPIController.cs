using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System.Linq;
using System.Threading.Tasks;
using IRepository.Live;
using Model.Live;
using AutoMapper;
using Entity.Live;
using Service;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;
using Model;
using Microsoft.IdentityModel.Tokens;
using IService;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZegoAPIController : ControllerBase
    {
        private readonly string _callbackSecret;
        private readonly IStreamSessionRepository _Repository;

        public ZegoAPIController(IConfiguration configuration, IStreamSessionRepository repository)
        {
            _callbackSecret = configuration["ApiSettings:CallbackSecret"];
            _Repository = repository;
        }
        [HttpPost("VerifyRoomClose")]
        public async Task<IActionResult> VerifyRoomClose([FromBody] RoomCloseRequest requestData)
        {

            try
            {
                if (requestData==null)
                {
                    return BadRequest("Request body is empty.");
                }

                // Kiểm tra các tham số cần thiết
                if (requestData == null ||
                    string.IsNullOrEmpty(requestData.Signature) ||
                    string.IsNullOrEmpty(requestData.Nonce) ||
                    requestData.Timestamp <= 0)
                {
                    return BadRequest("Invalid request data.");
                }

                // Tạo mảng chứa secret, timestamp và nonce
                string[] tempArr = { _callbackSecret, requestData.Timestamp.ToString(), requestData.Nonce };
                Array.Sort(tempArr);

                // Nối chuỗi
                string tmpStr = string.Join("", tempArr);

                // Tính SHA1
                string calculatedSignature = ComputeSha1(tmpStr);

                // So sánh kết quả
                if (calculatedSignature.Equals(requestData.Signature, StringComparison.OrdinalIgnoreCase))
                {
                    var session = await _Repository.getStreamSession(Guid.Parse(requestData.Room_id));
                    if (session != null) {
                        session.Status = 0;
                        session.EndTime = DateTime.Now;
                        await _Repository.UpdateStreamSessionAsync(session);
                        return Ok("End Live Success");
                    }               
                }

                return Unauthorized("Invalid signature.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string ComputeSha1(string input)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] tmpBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1.ComputeHash(tmpBytes);

                // Chuyển đổi băm sang dạng hex
                StringBuilder sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        // Class đại diện cho JSON payload
        public class RoomCloseRequest
        {
            public string Event { get; set; }
            public int AppId { get; set; }
            public long Timestamp { get; set; }
            public string Nonce { get; set; }
            public string Signature { get; set; }
            public string Room_id { get; set; }
            public long Room_session_id { get; set; }
            public int Close_reason { get; set; }
            public string Room_close_time { get; set; }
        }


    }
}