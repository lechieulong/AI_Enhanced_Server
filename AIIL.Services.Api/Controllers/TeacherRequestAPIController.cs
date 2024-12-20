using AutoMapper;
using Azure;
using Common;
using Entity;
using Entity.Data;
using IRepository;
using IRepository.Live;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using Repository;
using Repository.Live;
using System.Security.Claims;
using static Google.Apis.Requests.BatchRequest;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/teacherrequest")]
    [ApiController]
    public class TeacherRequestAPIController : ControllerBase
    {
        private readonly ITeacherRequestRepository _teacherRequestRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private ResponseDto _response;
        public TeacherRequestAPIController(ITeacherRequestRepository teacherRequestRepository, IMapper mapper, AppDbContext context)
        {
            _teacherRequestRepository = teacherRequestRepository;
            _mapper = mapper;
            _context = context;
            _response = new ResponseDto();
        }

        [HttpPost("beteacher")]
        [Authorize]
        public async Task<IActionResult> RegisterTeacher([FromBody] UserEducationDto userEducationDto)
        {
            try
            {
                // Lấy userId từ Claims
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound(new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found."
                    });
                }

                userEducationDto.TeacherId = userId;

                var teacherRequestDto = new TeacherRequestDto
                {
                    UserId = userId,
                    Description = "Request to become a teacher",
                    CreateAt = DateTime.Now,
                    Status = RequestStatusEnum.Pending
                };

                UserEducation userEducation = _mapper.Map<UserEducation>(userEducationDto);
                TeacherRequest teacherRequest = _mapper.Map<TeacherRequest>(teacherRequestDto);

                if (userEducationDto.SpecializationIds != null && userEducationDto.SpecializationIds.Any())
                {
                    var specializations = await _context.Specializations
                        .Where(s => userEducationDto.SpecializationIds.Contains(s.Id))
                        .ToListAsync();

                    userEducation.Specializations = specializations;
                }

                await _teacherRequestRepository.AddRequestAsync(teacherRequest, userEducation);

                // Tạo response DTO từ user education đã được thêm
                var resultDto = _mapper.Map<UserEducationDto>(userEducation);

                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Result = resultDto,
                    Message = "User education and teacher request created successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("random-admin-test")]
        [Authorize]
        public async Task<IActionResult> GetRandomAdminTest()
        {
            try
            {
                var testExam = await _teacherRequestRepository.GetRandomAdminTestAsync();

                if (testExam == null)
                {
                    _response.Message = "No test exam available.";
                    return BadRequest(_response);
                }
                _response.Result = testExam;
                _response.IsSuccess = true;
                _response.Message = "Get text exam successfully";
                return Ok(_response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while fetching the random admin test.",
                    Error = ex.Message
                });
            }
        }

        [HttpPost("update-request")]
        [Authorize]
        public async Task<IActionResult> UpdateTeacherRequest([FromBody] UserEducationDto userEducationDto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound(new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found."
                    });
                }

                userEducationDto.TeacherId = userId;

                UserEducation userEducation = _mapper.Map<UserEducation>(userEducationDto);

                var existingRequest = await _teacherRequestRepository.GetRequestByUserId(userId);
                if (existingRequest == null)
                {
                    return NotFound(new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Teacher request not found."
                    });
                }

                await _teacherRequestRepository.UpdateRequestAsync(existingRequest, userEducation);

                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Teacher request and user education updated successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Internal server error: {ex.Message}"
                });
            }
        }

        [HttpGet("teacher-request")]
        [Authorize]
        public async Task<IActionResult> GetTeacherRequest()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return NotFound(new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "User not found."
                    });
                }

                var teacherRequest = await _teacherRequestRepository.GetRequestByUserId(userId);
                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Result = teacherRequest,
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

        [HttpGet("get-requests")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllTeacherRequests(RequestStatusEnum status, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and page size must be greater than zero.");
            }
            try
            {
                var (requests, totalCount) = await _teacherRequestRepository.GetTeacherRequestsAsync(page, pageSize, status);

                // Đóng gói dữ liệu trả về
                _response.Result = new
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize), // Calculate total pages safely
                    TotalCount = totalCount,
                    Requests = requests,
                };
                _response.IsSuccess = true;
                _response.Message = "Data retrieved successfully";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Internal server error: {ex.Message}";

                return StatusCode(500, _response);
            }
        }


        [HttpGet("get-request-by-id/{requestId}")]
        [Authorize]
        public async Task<IActionResult> GetTeacherRequestByRequestId(Guid requestId)
        {
            var requestData = await _teacherRequestRepository.GetRequestByRequestId(requestId);

            if (requestData == null)
            {
                _response.IsSuccess = false ;
                _response.Message = "Teacher request not found.";
                _response.Result = null;
                return NotFound(_response);
            }

            _response.IsSuccess = true;
            _response.Message = "Get request successful";
            _response.Result = requestData;
            return Ok(_response);
        }

        [HttpPost("process-request/{requestId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> ProcessTeacherRequest(Guid requestId, [FromBody] ProcessTeacherRequestDto processTeacherRequestDto)
        {
            if (processTeacherRequestDto == null)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid request data."
                });
            }

            try
            {
                var updatedRequest = await _teacherRequestRepository.ProcessRequest(requestId, processTeacherRequestDto);

                if (updatedRequest == null)
                {
                    return NotFound(new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Teacher request not found."
                    });
                }

                _response.IsSuccess = true;
                _response.Message = "Teacher request processed successfully.";
                _response.Result = updatedRequest;

                return Ok(_response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Teacher request not found."
                });
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Internal server error: {ex.Message}";

                return StatusCode(500, _response);
            }
        }

    }
}
