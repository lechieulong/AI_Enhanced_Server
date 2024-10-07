using AutoMapper;
using Entity;
using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Repository;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/teacheravailableschedule")]
    [ApiController]
    public class TeacherAvailableScheduleAPIController : ControllerBase
    {
        private readonly ITeacherScheduleRepository _teacherScheduleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private ResponseDto _response;
        public TeacherAvailableScheduleAPIController(ITeacherScheduleRepository teacherScheduleRepository, IUserRepository userRepository, IMapper mapper)
        {
            _teacherScheduleRepository = teacherScheduleRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _response = new ResponseDto();
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
        [Route("{id:int}")]
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
        //[Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Post([FromBody] TeacherAvailableScheduleDto scheduleDto)
        {
            try
            {
                TeacherAvailableSchedule schedule = _mapper.Map<TeacherAvailableSchedule>(scheduleDto);
                schedule = await _teacherScheduleRepository.CreateAsync(schedule);
                _response.Result = _mapper.Map<TeacherAvailableScheduleDto>(schedule);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Put([FromBody] TeacherAvailableScheduleDto scheduleDto)
        {
            try
            {
                TeacherAvailableSchedule schedule = _mapper.Map<TeacherAvailableSchedule>(scheduleDto);
                schedule = await _teacherScheduleRepository.UpdateAsync(schedule);
                _response.Result = _mapper.Map<TeacherAvailableScheduleDto>(schedule);
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
                bool isDeleted = await _teacherScheduleRepository.DeleteAsync(id);
                if (!isDeleted)
                {
                    _response.IsSuccess = false;
                    _response.Message = "schedule not found";
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
        [Route("{userName}")]
        public async Task<ResponseDto> GetScheduleByTeacherId(string userName)
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
                IEnumerable<TeacherAvailableSchedule> scheduleList = await _teacherScheduleRepository.GetByTeacherIdAsync(userName);
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
