using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System.Linq;
using System.Threading.Tasks;
using IRepository.Live;
using Model.Live;
using AutoMapper;
using Entity.Live;
using Model.Payment;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftRepository _repository;
        private readonly IMapper _Mapper;

        public GiftController(IGiftRepository repository, IMapper Mapper)
        {
            _repository = repository;
            _Mapper = Mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var model = await _repository.GetGiftAsync();
            var result = _Mapper.Map<List<GiftModel>>(model);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGiftById(Guid id)
        {
            var model = await _repository.GetGiftByIDAsync(id);
            var result = _Mapper.Map<GiftModel>(model);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateGift([FromBody] GiftModel model)
        {
            if (model == null)
            {
                return BadRequest(model);
            }
            var gift = _Mapper.Map<Gift>(model);
            var result = await _repository.AddGiftAsync(gift);
            return Ok(_Mapper.Map<GiftModel>(result));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateGiftce([FromBody] GiftModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }
            var gift = _Mapper.Map<Gift>(model);
            var result = await _repository.UpdateGiftAsync(gift);
            return Ok(_Mapper.Map<GiftModel>(result));
        }
    }
}