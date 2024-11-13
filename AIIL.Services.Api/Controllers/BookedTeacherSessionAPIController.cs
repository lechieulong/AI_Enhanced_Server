using AutoMapper;
using Entity;
using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public BookedTeacherSessionAPIController(IMapper mapper, IBookedScheduleSessionRepository bookedScheduleSessionRepository, IEventRepository eventRepository)
        {
            _bookedScheduleSessionRepository = bookedScheduleSessionRepository;
            _mapper = mapper;
            _response = new ResponseDto();
            _eventRepository = eventRepository;
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
    }
}
