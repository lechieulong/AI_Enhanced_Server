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
    [Route("api/teacheravailableschedule")]
    [ApiController]
    public class TeacherAvailableScheduleAPIController : ControllerBase
    {
        private readonly ITeacherScheduleRepository _teacherScheduleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ResponseDto _response;
        public TeacherAvailableScheduleAPIController(ITeacherScheduleRepository teacherScheduleRepository, IUserRepository userRepository, IMapper mapper)
        {
            _teacherScheduleRepository = teacherScheduleRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _response = new();
        }
        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<TeacherAvailableSchedule> scheduleList = await _teacherScheduleRepository.GetAllAsync();
                _response.Result = _mapper.Map<IEnumerable<TeacherAvailableScheduleDto>>(scheduleList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("getbyid/{id:int}")]
        public async Task<ResponseDto> Get(Guid id)
        {
            try
            {
                TeacherAvailableSchedule schedule = await _teacherScheduleRepository.GetByIdAsync(id);
                _response.Result = _mapper.Map<TeacherAvailableScheduleDto>(schedule);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpPost]
        [Authorize(Roles = "TEACHER")]
        public async Task<IActionResult> Post([FromBody] TeacherAvailableScheduleDto scheduleDto)
        {
            try
            {
                var teacherId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                // Validate the teacher ID
                if (string.IsNullOrEmpty(teacherId))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Teacher ID is missing.";
                    return BadRequest(_response);
                }

                // Check for schedule conflicts
                var conflictingSchedules = await _teacherScheduleRepository.GetConflictingSchedulesAsync(
                    teacherId, scheduleDto.StartTime, scheduleDto.EndTime);
                if (conflictingSchedules.Any())
                {
                    var firstConflict = conflictingSchedules.FirstOrDefault();

                    _response.IsSuccess = false;
                    _response.Message = $"You already have a schedule starting at {firstConflict.StartTime} for {firstConflict.Minutes} minutes.";
                    return Conflict(_response);
                }

                TeacherAvailableSchedule schedule = _mapper.Map<TeacherAvailableSchedule>(scheduleDto);
                schedule.TeacherId = teacherId;
                schedule = await _teacherScheduleRepository.CreateAsync(schedule);
                _response.Result = _mapper.Map<TeacherAvailableScheduleDto>(schedule);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPatch]
        [Authorize(Roles = "TEACHER")]
        public async Task<IActionResult> Patch([FromBody] UpdateScheduleDto scheduleDto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "User is not authenticated.";
                    return Unauthorized(_response); // User not authenticated
                }

                var schedule = await _teacherScheduleRepository.GetByIdAsync(scheduleDto.Id);
                if (schedule == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Schedule not found.";
                    return NotFound(_response);
                }

                if(userId != scheduleDto.TeacherId)
                {
                    _response.IsSuccess = false;
                    _response.Message = "You can not edit this schedule.";
                    return BadRequest(_response);
                }

                switch (schedule.Status)
                {
                    case 1: // Pending
                        _response.IsSuccess = false;
                        _response.Message = "Cannot update schedule. It is currently pending.";
                        return BadRequest(_response);

                    case 2: // Booked
                        _response.IsSuccess = false;
                        _response.Message = "Cannot update schedule. It is already booked.";
                        return BadRequest(_response);
                }

                // Check for schedule conflicts
                var conflictingSchedules = await _teacherScheduleRepository.GetConflictingSchedulesAsync(
                    scheduleDto.TeacherId, scheduleDto.StartTime, scheduleDto.EndTime, scheduleDto.Id);
                if (conflictingSchedules.Any())
                {
                    var firstConflict = conflictingSchedules.FirstOrDefault();

                    _response.IsSuccess = false;
                    _response.Message = $"You already have a schedule starting at {firstConflict.StartTime} for {firstConflict.Minutes} minutes.";
                    return Conflict(_response);
                }

                TeacherAvailableSchedule scheduleEntity = _mapper.Map<TeacherAvailableSchedule>(scheduleDto);
                scheduleEntity = await _teacherScheduleRepository.UpdateAsync(scheduleEntity);

                _response.IsSuccess = true;
                _response.Message = "Schedule updated successfully.";
                _response.Result = _mapper.Map<UpdateScheduleDto>(scheduleEntity);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = "An error occurred while updating the schedule. Please try again.";
            }
            return Ok(_response);
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "TEACHER")]
        public async Task<ResponseDto> Delete(Guid id)
        {
            try
            {
                var schedule = await _teacherScheduleRepository.GetByIdAsync(id);
                if (schedule == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Schedule not found.";
                    return _response;
                }
                switch (schedule.Status)
                {
                    case 1: // Pending
                        _response.IsSuccess = false;
                        _response.Message = "Cannot delete schedule. It is currently pending.";
                        return _response;

                    case 2: // Booked
                        _response.IsSuccess = false;
                        _response.Message = "Cannot delete schedule. It is already booked.";
                        return _response;
                }
                bool isDeleted = await _teacherScheduleRepository.DeleteAsync(id);
                if (!isDeleted)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Failed to delete schedule.";
                    return _response;
                }
                _response.IsSuccess = true;
                _response.Message = "Schedule deleted successfully.";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("getbyusername/{userName}")]
        public async Task<ResponseDto> GetScheduleByTeacherName(string userName)
        {
            try
            {
                UserDto user = await _userRepository.GetUserProfileByUsernameAsync(userName);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "User not found.";
                    return _response;
                }
                IEnumerable<TeacherAvailableSchedule> scheduleList = await _teacherScheduleRepository.GetByTeacherNameAsync(userName);
                if (scheduleList != null)
                {
                    _response.Result = _mapper.Map<IEnumerable<TeacherAvailableScheduleDto>>(scheduleList);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Schedule not found for this teacher.";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("{userName}/7days")]
        public async Task<ResponseDto> GetScheduleByTeacherName7Days(string userName)
        {
            try
            {
                UserDto user = await _userRepository.GetUserProfileByUsernameAsync(userName);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "User not found.";
                    return _response;
                }
                IEnumerable<TeacherAvailableSchedule> scheduleList = await _teacherScheduleRepository.GetByTeacherName7DaysAsync(userName);
                if (scheduleList != null)
                {
                    _response.Result = _mapper.Map<IEnumerable<TeacherAvailableScheduleDto>>(scheduleList);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Schedule not found for this teacher.";
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
