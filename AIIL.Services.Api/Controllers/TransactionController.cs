using IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using IRepository.Live;
using Model.Live;
using AutoMapper;
using Entity.Live;
using Model.Payment;
using Entity.Payment;

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
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions(int page, int pageSize, string? searchQuery)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and page size must be greater than zero.");
            }

            var (Transactions, totalCount) = await _repository.GetTransactionAsyn(page, pageSize, searchQuery);

            // Ensure the total count is always non-negative
            totalCount = totalCount < 0 ? 0 : totalCount;

            var response = new
            {
                Transactions = _Mapper.Map<List<TransactionModel>>(Transactions),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize) // Calculate total pages safely
            };

            return Ok(response);
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
