using AutoMapper;
using Entity;
using Entity.Data;
using IRepository;
using IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Utility;
using OfficeOpenXml;
using Repository;
using Service;
using System.Security.Claims;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IBlogStorageService _blobStorageService;
        private readonly ResponseDto _response;
        private readonly AppDbContext _db;
        IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAPIController(IUserRepository userRepository, IMapper mapper, AppDbContext db, UserManager<ApplicationUser> userManager, IBlogStorageService blogStorageService, IAuthRepository authRepository, IEmailSenderService emailSenderService)
        {
            _userRepository = userRepository;
            _response = new ResponseDto();
            _mapper = mapper;
            _db = db;
            _userManager = userManager;
            _blobStorageService = blogStorageService;
            _authRepository = authRepository;
            _emailSenderService = emailSenderService;
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

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto userDto)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId == null)
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.Message = "User not found";
                return NotFound(_response);
            }
            try
            {
                var existingUser = await _db.ApplicationUsers.FindAsync(currentUserId);
                if (existingUser == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = null;
                    _response.Message = "User not found";
                    return NotFound(_response);
                }
                _mapper.Map(userDto, existingUser);

                _db.ApplicationUsers.Update(existingUser);
                await _db.SaveChangesAsync();

                _response.IsSuccess = true;
                _response.Result = _mapper.Map<UserDto>(existingUser);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Error updating profile: {ex.Message}";
                return StatusCode(500, _response); // Return server error status code
            }
            return Ok(_response);
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
            _response.Result = topTeachers;

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

        [HttpPost("unlock/{userId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _response.IsSuccess = false;
                _response.Message = "User ID is required.";
                return BadRequest(_response);
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                _response.IsSuccess = false;
                _response.Message = "User not found.";
                return NotFound(_response);
            }

            await _userRepository.UnlockUserAsync(user.Id);
            _response.IsSuccess = true;
            _response.Message = "User unlocked successfully.";

            return Ok(_response);
        }

        [HttpPost("import-excel")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _response.IsSuccess = false;
                _response.Message = "Please upload a valid Excel file.";
                return BadRequest(_response);
            }

            var importedUsers = new List<UserDto>();
            var errors = new List<string>();

            try
            {
                // Call the repository method to parse the file and get users
                var users = await _userRepository.ImportUserAsync(file);
                foreach (var userDto in users)
                {
                    var existingUserByUsername = await _userManager.FindByNameAsync(userDto.UserName);
                    var existingUserByEmail = await _userManager.FindByEmailAsync(userDto.Email);
                    if (existingUserByUsername != null || existingUserByEmail != null)
                    {
                        // Handle the case where the user already exists
                        string errorMessage = existingUserByUsername != null
                            ? $"Username '{userDto.UserName}' is already taken."
                            : $"Email '{userDto.Email}' is already registered.";
                        errors.Add(errorMessage); // Add error message to the list
                        continue; // Skip to the next user
                    }
                    // Map UserFromFileDto to ApplicationUser
                    var user = _mapper.Map<ApplicationUser>(userDto);
                    // Attempt to create the user with a password
                    var result = await _userManager.CreateAsync(user, userDto.Password);
                    if (result.Succeeded)
                    {
                        await _authRepository.AssignRole(user.Email, SD.User);
                        // Fetch the created user to return a UserDto
                        var userToReturn = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == user.Email);
                        if (userToReturn != null)
                        {
                            importedUsers.Add(new UserDto
                            {
                                ID = userToReturn.Id,
                                Email = userToReturn.Email,
                                Name = userToReturn.Name,
                                PhoneNumber = userToReturn.PhoneNumber
                            });
                            await _emailSenderService.SendRegistrationSuccessEmail(user.Email, user.Name, user.UserName);
                        }
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            errors.Add(error.Description);
                        }
                    }
                }
                // Set the response based on the outcome
                if (errors.Any())
                {
                    _response.IsSuccess = true;
                    _response.Message = "Some users could not be imported.";
                    _response.Result = new
                    {
                        ImportedUsers = importedUsers,
                        ImportedCount = importedUsers.Count,
                        Errors = errors
                    };

                    return Ok(_response); // Ensure you return the response if there are errors
                }
                else
                {
                    _response.IsSuccess = true;
                    _response.Message = "All users imported successfully.";
                    _response.Result = new
                    {
                        ImportedUsers = importedUsers,
                        ImportedCount = importedUsers.Count,
                        Errors = errors
                    };

                    return Ok(_response); // Return the successful response
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "An error occurred while importing users.";
                _response.Result = new { Details = ex.Message };
                return StatusCode(500, _response);
            }

        }

        [HttpGet("usereducation/{username}")]
        public async Task<IActionResult> GetUserEducation(string username)
        {
            try
            {
                var userEducation = await _userRepository.GetUserEducationByUSerName(username);
                if (userEducation == null)
                {
                    return NotFound(new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found."
                    });
                }
                if (userEducation?.UserEducation == null)
                {
                    return NotFound(new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "User education data not found."
                    });
                }

                var userEducationDto = _mapper.Map<UserDto>(userEducation);

                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Result = userEducationDto,
                    Message = ""
                });
            }
            catch (KeyNotFoundException ex)
            {
                // Return 404 if no request is found for the user
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Return 500 for other errors
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("update-role")]
        [Authorize]
        public async Task<IActionResult> UpdateRole()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                _response.IsSuccess = false;
                _response.Message = "User not found.";
                return NotFound(_response);
            }

            try
            {
                var token = await _userRepository.UpdateRole(userId);

                if (token == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Can't update role";
                    return BadRequest(_response);
                }

                _response.IsSuccess = true;
                _response.Message = "Update role succesful";
                _response.Result = token;
                return Ok(_response);
            }
            catch (KeyNotFoundException ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"User not found: {ex.Message}";
                return NotFound(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, _response);
            }
        }
    }
}
