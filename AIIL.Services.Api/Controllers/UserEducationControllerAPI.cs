using AutoMapper;
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
    [Route("api/usereducation")]
    [ApiController]
    public class UserEducationControllerAPI : ControllerBase
    {
        private readonly IUserEducationRepository _userEducationRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private ResponseDto _response;
        public UserEducationControllerAPI(IUserEducationRepository userEducationRepository, IMapper mapper, AppDbContext context)
        {
            _userEducationRepository = userEducationRepository;
            _mapper = mapper;
            _context = context;
            _response = new ResponseDto();
        }

        [HttpPost("beteacher")]
        public async Task<IActionResult> RegisterTeacher([FromBody] UserEducationDto userEducationDto)
        {
            try
            {
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

                UserEducation education = _mapper.Map<UserEducation>(userEducationDto);

                if (userEducationDto.SpecializationIds != null && userEducationDto.SpecializationIds.Any())
                {
                    var specializations = await _context.Specializations
                        .Where(s => userEducationDto.SpecializationIds.Contains(s.Id))
                        .ToListAsync();

                    education.Specializations = specializations;
                }

                // Tạo mới giáo viên (UserEducation) với các chuyên môn đã liên kết
                education = await _userEducationRepository.CreateAsync(education);

                var resultDto = _mapper.Map<UserEducationDto>(education);
                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Result = resultDto,
                    Message = "User education created successfully."
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
