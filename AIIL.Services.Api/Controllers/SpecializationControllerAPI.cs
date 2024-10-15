using AutoMapper;
using Entity;
using Entity.Data;
using IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Repository;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/specialization")]
    [ApiController]
    public class SpecializationControllerAPI : ControllerBase
    {
        private readonly ISpecializationRepository _specializationRepository;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private ResponseDto _response;

        public SpecializationControllerAPI(ISpecializationRepository specializationRepository, IMapper mapper, AppDbContext context)
        {
            _specializationRepository = specializationRepository;
            _mapper = mapper;
            _context = context;
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<Specialization> specializations = await _specializationRepository.GetAllAsync();
                _response.Result = _mapper.Map<IEnumerable<SpecializationDto>>(specializations);
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
