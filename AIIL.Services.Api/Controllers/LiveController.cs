using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System.Linq;
using System.Threading.Tasks;
using IRepository.Live;
using AutoMapper;
using Model.Live;
using Entity.Live;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiveController : ControllerBase
    {
        private readonly ILiveStreamRepository _repository;
        private readonly IMapper _Mapper;

        public LiveController(ILiveStreamRepository repository, IMapper Mapper)
        {
            _repository = repository;
            _Mapper = Mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var live = await _repository.GetLiveStreamsAsync();
            var result = _Mapper.Map<List<LiveStreamModel>>(live);
            return Ok(result);
        }
        [HttpGet("{liveid}")]
        public async Task<IActionResult> getLive(Guid liveid)
        {
            var live = await _repository.GetLiveStreamBtIdAsync(liveid);
            var result = _Mapper.Map<LiveStreamModel>(live);
            return Ok(result);
        }
        [HttpGet("lives")]
        public async Task<IActionResult> GetLivesBlock(int page, int pageSize, string? searchQuery)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and page size must be greater than zero.");
            }

            var (Live, totalCount) = await _repository.GetLivesBlockAsync(page, pageSize, searchQuery);

            // Ensure the total count is always non-negative
            totalCount = totalCount < 0 ? 0 : totalCount;

            var response = new
            {
                Lives = Live,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize) // Calculate total pages safely
            };

            return Ok(response);
        }

        [HttpPost("{liveid}")]
        public async Task<IActionResult> CreateLive(Guid id)
        {
            var live = await _repository.GetLiveStreamBtIdAsync(id);
            if (live != null) {

                var result = _Mapper.Map<LiveStreamModel>(live);
                return Ok(result);
            }
            else
            {
            var result = await _repository.AddLiveStreamAsync(id);
            return Ok(_Mapper.Map<LiveStreamModel>(result));
            }
            
        }
        [HttpPut]
        public async Task<IActionResult> UpdateStreamSession(LiveStreamModel model)
        {
            if (model == null)
            {
                return BadRequest(model);
            }
            model.UpdateDate = DateTime.Now;
            var live = _Mapper.Map<LiveStream>(model);
            var result = await _repository.UpdateLiveStreamAsync(live);
            return Ok(model);
        }
    }
}
