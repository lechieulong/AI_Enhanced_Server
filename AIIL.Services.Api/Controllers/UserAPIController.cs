using AutoMapper;
using Entity;
using Entity.Data;
using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAPIController(IUserRepository userRepository, IMapper mapper, AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _response = new ResponseDto();
            _mapper = mapper;
            _db = db;
            _userManager = userManager;
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

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _response.IsSuccess = false;
                _response.Message = "UserId is required.";
                return BadRequest(_response);
            }

            var userProfile = await _userRepository.GetUserUserByIdAsync(userId);

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
            var currentUserId = _userManager.GetUserId(User);
            var topTeachers = await _userRepository.GetTopTeachersAsync(currentUserId);

            if (topTeachers == null || !topTeachers.Any())
            {
                _response.IsSuccess = true;
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

        [HttpGet("users")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetUsers(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and page size must be greater than zero.");
            }

            var (users, totalCount) = await _userRepository.GetUsersAsync(page, pageSize);

            // Ensure the total count is always non-negative
            totalCount = totalCount < 0 ? 0 : totalCount;

            var response = new
            {
                Users = users,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize) // Calculate total pages safely
            };

            return Ok(response);
        }


        [HttpPost("lock")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> LockUser([FromBody] LockUserRequestDto request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || request.DurationInMinutes <= 0)
            {
                return BadRequest("Invalid request data.");
            }

            var user = await _userRepository.GetUserByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            await _userRepository.LockUserAsync(request.UserId, request.DurationInMinutes);
            return Ok("User locked successfully.");
        }
    }
}
