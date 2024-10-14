using AutoMapper;
using Entity;
using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Repository;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/usereducation")]
    [ApiController]
    public class UserEducationControllerAPI : ControllerBase
    {
        private readonly IUserEducationRepository _userEducationRepository;
        private readonly IMapper _mapper;
        private ResponseDto _response;
        public UserEducationControllerAPI(IUserEducationRepository userEducationRepository, IMapper mapper)
        {
            _userEducationRepository = userEducationRepository;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        [HttpPost]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ResponseDto> Post([FromBody] UserEducationDto userEducationDto)
        {
            try
            {
                UserEducation education = _mapper.Map<UserEducation>(userEducationDto);
                education = await _userEducationRepository.CreateAsync(education);
                _response.Result = _mapper.Map<EventDto>(education);
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
