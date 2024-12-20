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
using Microsoft.AspNetCore.Identity;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountBalanceController : ControllerBase
    {
        private readonly IAccountBalanceRepository _repository;
        private readonly IMapper _Mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountBalanceController(IAccountBalanceRepository repository, IMapper Mapper, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _Mapper = Mapper;
            _userManager = userManager;
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

        [HttpGet("Get-balance-history-by-userid")]
        [Authorize]
        public async Task<IActionResult> GetBalanceHistoryByUserId()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return NotFound("User not found.");
            }

            var history = await _repository.GetBalanceHistoryByUserIdAsync(currentUserId);
            if (history == null || !history.Any())
            {
                return NotFound("No balance history found for the user.");
            }

            // Map to DTO
            var historyDto = history.Select(h => new BalanceHistoryDto
            {
                Id = h.Id,
                Amount = h.amount,
                Description = h.Description,
                CreateDate = h.CreateDate
            });

            return Ok(historyDto);
        }

    }
}
