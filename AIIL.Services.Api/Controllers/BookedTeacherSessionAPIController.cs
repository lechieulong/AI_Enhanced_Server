using AutoMapper;
using Entity;
using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Model;
using Repository;
using System.Security.Claims;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/bookedschedulesession")]
    [ApiController]
    public class BookedTeacherSessionAPIController : ControllerBase
    {
        private readonly IMapper _mapper;
        private ResponseDto _response;
        private readonly IBookedScheduleSessionRepository _bookedScheduleSessionRepository;
        private readonly IEventRepository _eventRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookedTeacherSessionAPIController(IMapper mapper, IBookedScheduleSessionRepository bookedScheduleSessionRepository, IEventRepository eventRepository, UserManager<ApplicationUser> userManager)
        {
            _bookedScheduleSessionRepository = bookedScheduleSessionRepository;
            _mapper = mapper;
            _response = new ResponseDto();
            _eventRepository = eventRepository;
            _userManager = userManager;
        }

        [HttpPost("create-schedule-session")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] BookedTeacherSessionDto bookedTeacherSessionDto)
        {
            try
            {
                var bookedSession = _mapper.Map<BookedTeacherSession>(bookedTeacherSessionDto);
                bookedSession = await _bookedScheduleSessionRepository.CreateScheduleSessionAsync(bookedSession);

                var eventEntity = new Event
                {
                    Title = "Booked Coaching Session",
                    Description = "A Coaching session booked with a teacher",
                    Start = bookedSession.TeacherAvailableSchedule.StartTime,
                    End = bookedSession.TeacherAvailableSchedule.EndTime,
                    Link = bookedSession.TeacherAvailableSchedule.Link
                };

                await _eventRepository.CreateEventAsync(eventEntity, new List<string> { bookedTeacherSessionDto.LearnerId, bookedSession.TeacherAvailableSchedule.TeacherId });

                var responseDto = _mapper.Map<BookedTeacherSessionDto>(bookedSession);
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("getSessionsByUserId")]
        [Authorize]
        public async Task<IActionResult> GetSessionsByUserId()
        {
            try
            {
                var currentUserId = _userManager.GetUserId(User);

                if (string.IsNullOrEmpty(currentUserId))
                {
                    _response.IsSuccess = false;
                    _response.Message = "User not found.";
                    return NotFound(_response);
                }

                var sessions = await _bookedScheduleSessionRepository.GetSessionsByUserIdAsync(currentUserId);

                if (sessions == null || !sessions.Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "No sessions found for this user.";
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<IEnumerable<GetSessionsByUserIdDto>>(sessions);
                _response.IsSuccess = true;
                _response.Message = "Get sessions successful";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"An error occurred: {ex.Message}";
                return StatusCode(500, _response); // Internal Server Error
            }
        }

    }
}
