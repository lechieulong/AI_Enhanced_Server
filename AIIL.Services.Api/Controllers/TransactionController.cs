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
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _Mapper;

        public TransactionController(ITransactionRepository repository, IMapper Mapper)
        {
            _repository = repository;
            _Mapper = Mapper; 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var Transaction = await _repository.GetTransactionByIdAsync(id);
            return Ok(Transaction);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }
            var Transaction=_Mapper.Map<Transaction>(model);

            var result = await _repository.AddTransactionAsync(model);
            return Ok(result);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateTransaction([FromBody] TransactionModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }
            var Transaction = _Mapper.Map<Transaction>(model);

            var result = await _repository.UpdateTransactionAsync(model);
            return Ok(result);
        }
    }
}
