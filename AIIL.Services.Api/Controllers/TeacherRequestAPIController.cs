using AutoMapper;
using Common;
using Entity;
using Entity.Data;
using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using Repository;
using System.Security.Claims;

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
        public async Task<IActionResult> RegisterTeacher([FromBody] UserEducationDto userEducationDto)
        {
            try
            {
                // Lấy userId từ Claims
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new ResponseDto
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
                    CreateAt = DateTime.UtcNow,
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

    }
}
