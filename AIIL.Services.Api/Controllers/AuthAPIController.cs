using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.LoginGoogle;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly ResponseDto _response;
        public AuthAPIController(IAuthRepository authService)
        {
            _authRepository = authService;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authRepository.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authRepository.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Login Failed!";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authRepository.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error Encountered.";
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("check-email")]
        public async Task<IActionResult> CheckEmailExists([FromBody] CheckEmailRequestDto model)
        {
            var emailExists = await _authRepository.CheckEmailExists(model.Email);
            _response.IsSuccess = emailExists;
            _response.Message = emailExists ? "Email exists." : "Email does not exist.";
            return Ok(_response);
        }

        [HttpPost("register-google")]
        public async Task<IActionResult> RegisterGoogleUser([FromBody] GoogleRegistrationRequestDto model)
        {
            var errorMessage = await _authRepository.RegisterWithGoogle(model.Email, model.Token);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginRequestDto model)
        {
            var loginResponse = await _authRepository.LoginWithGoogle(model.Token);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Google Login Failed!";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }
    }
}
