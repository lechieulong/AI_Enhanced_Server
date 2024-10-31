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
    public class AccountBalanceController : ControllerBase
    {
        private readonly IAccountBalanceRepository _repository;
        private readonly IMapper _Mapper;

        public AccountBalanceController(IAccountBalanceRepository repository, IMapper Mapper)
        {
            _repository = repository;
            _Mapper = Mapper; 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountBalanceByUserId(String id)
        {
            var model = await _repository.GetBalace(id);
            return Ok(model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAccountBalance(String id)
        {
            var model = await _repository.GetBalace(id);
            return Ok(model);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAccountBalance([FromBody] AccountBalaceModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var result = await _repository.UpdateBalanceAsync(model);
            return Ok(result);
        }
    }
}
