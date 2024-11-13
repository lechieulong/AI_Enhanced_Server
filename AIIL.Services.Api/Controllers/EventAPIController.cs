using AutoMapper;
using Entity;
using IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/event")]
    [ApiController]
    public class EventAPIController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private ResponseDto _response;

        public EventAPIController(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<Event> eventList = await _eventRepository.GetEventsAsync();
                _response.Result = _mapper.Map<IEnumerable<EventDto>>(eventList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ResponseDto> Get(Guid id)
        {
            try
            {
                Event Event = await _eventRepository.GetEventsByIdAsync(id);
                _response.Result = _mapper.Map<EventDto>(Event);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        //[HttpGet]
        //[Route("{userId}")]
        //public async Task<ResponseDto> GetEventByUserId(string userId)
        //{
        //    try
        //    {
        //        Event eventObj = await _eventRepository.GetEventByUserIdAsync(userId);
        //        if (eventObj != null)
        //        {
        //            _response.Result = _mapper.Map<EventDto>(eventObj);
        //        }
        //        else
        //        {
        //            _response.IsSuccess = false;
        //            _response.Message = "Event not found for this user.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = ex.Message;
        //    }
        //    return _response;
        //}
        [HttpGet]
        [Route("events")]
        [Authorize]
        public async Task<IActionResult> GetEventsForCurrentUser()
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    IEnumerable<Event> events = await _eventRepository.GetEventsByUserIdAsync(userId);
                    _response.Result = _mapper.Map<IEnumerable<EventDto>>(events);
                    _response.IsSuccess = true;
                    _response.Message = "Get event successful";
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "User not found.";
                    return NotFound(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return BadRequest(_response);
        }


        [HttpPost("create-event")]
        //[Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Post([FromBody] EventDto eventDto)
        {
            try
            {
                Event Event = _mapper.Map<Event>(eventDto);
                Event = await _eventRepository.CreateEventAsync(Event, eventDto.UserIds);
                _response.Result = _mapper.Map<EventDto>(Event);
            }
            
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }

            return Ok(_response);
        }


        [HttpPut]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Put([FromBody] EventDto EventDto)
        {
            try
            {
                Event Event = _mapper.Map<Event>(EventDto);
                Event = await _eventRepository.UpdateEventAsync(Event);
                _response.Result = _mapper.Map<EventDto>(Event);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Delete(Guid id)
        {
            try
            {
                bool isDeleted = await _eventRepository.DeleteEventAsync(id);
                if (!isDeleted)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Event not found";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
