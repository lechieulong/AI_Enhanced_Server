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
using Microsoft.AspNetCore.Authorization;
using Repository;

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
            gift.CreatedDate = DateTime.Now;
            var result = await _repository.AddGiftAsync(gift);
            return Ok(_Mapper.Map<GiftModel>(result));
        }
        [HttpPut]
        public async Task<IActionResult> UpdateGift([FromBody] GiftModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }
            var gift = _Mapper.Map<Gift>(model);
            var result = await _repository.UpdateGiftAsync(gift);
            return Ok(_Mapper.Map<GiftModel>(result));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGift(Guid id)
        {
            var result = await _repository.DeleteAsync(id);
            return Ok(_Mapper.Map<GiftModel>(result));
        }

        [HttpGet("gifts")]
        public async Task<IActionResult> GetGifts(int page, int pageSize, string? searchQuery)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and page size must be greater than zero.");
            }

            var (gifts, totalCount) = await _repository.GetUsersAsync(page, pageSize, searchQuery);

            // Ensure the total count is always non-negative
            totalCount = totalCount < 0 ? 0 : totalCount;

            var response = new
            {
                gifts = _Mapper.Map<List<GiftModel>>(gifts),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize) // Calculate total pages safely
            };

            return Ok(response);
        }
    }
}