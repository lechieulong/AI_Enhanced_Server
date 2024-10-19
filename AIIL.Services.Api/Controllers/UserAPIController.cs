using AutoMapper;
using Entity;
using Entity.Data;
using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ResponseDto _response;
        private readonly AppDbContext _db;
        IMapper _mapper;

        public UserAPIController(IUserRepository userRepository, IMapper mapper, AppDbContext db)
        {
            _userRepository = userRepository;
            _response = new ResponseDto();
            _mapper = mapper;
            _db = db;
        }

        [HttpGet("profile/{username}")]
        public async Task<IActionResult> GetUserProfile(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                _response.IsSuccess = false;
                _response.Message = "Username is required.";
                return BadRequest(_response);
            }

            var userProfile = await _userRepository.GetUserProfileByUsernameAsync(username);

            if (userProfile == null)
            {
                _response.IsSuccess = false;
                _response.Message = "User not found.";
                return NotFound(_response);
            }

            _response.IsSuccess = true;
            _response.Result = userProfile;
            return Ok(_response);
        }

        [HttpPut]
        //[Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] UserDto userDto)
        {
            try
            {
                ApplicationUser obj = _mapper.Map<ApplicationUser>(userDto);
                _db.ApplicationUsers.Update(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<UserDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet("top-teachers")]
        public async Task<IActionResult> GetTopTeachers()
        {

            var topTeachers = await _userRepository.GetTopTeachersAsync();

            if (topTeachers == null || !topTeachers.Any())
            {
                _response.IsSuccess = false;
                _response.Message = "No teachers found.";
                return NotFound(_response);
            }

            _response.IsSuccess = true;
            _response.Result = topTeachers.Select(u => new UserDto
            {
                ID = u.Id,
                UserName = u.UserName,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                ImageURL = u.ImageURL
            }).ToList(); // Return a list of UserDto

            return Ok(_response);
        }

        [HttpGet]
        [Route("search/{searchText}")]
        public async Task<IActionResult> SearchTeachers(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return BadRequest("Search text is required.");
            }

            var teachers = await _userRepository.SearchTeachersAsync(searchText);

            if (teachers == null || !teachers.Any())
            {
                return NotFound("No teachers found.");
            }

            return Ok(teachers);
        }


    }
}
