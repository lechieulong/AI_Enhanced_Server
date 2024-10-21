using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System.Linq;
using System.Threading.Tasks;
using IRepository.Live;
using AutoMapper;
using Model.Live;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StreamSessionController : ControllerBase
    {
        private readonly IStreamSessionRepository _repository;
        private readonly IMapper _Mapper;

        public StreamSessionController(IStreamSessionRepository repository, IMapper Mapper)
        {
            _repository = repository;
            _Mapper = Mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var Streamsession = await _repository.getStreamSessionsIsLive();
            var result = _Mapper.Map<List<StreamSessionModel>>(Streamsession);
            return Ok(result);
        }       
    }
}
