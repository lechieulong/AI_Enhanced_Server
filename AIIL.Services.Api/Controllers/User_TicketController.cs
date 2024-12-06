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
using Repository.Live;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class User_TicketController : ControllerBase
    {
        private readonly IUser_TicketRepository _userTicketRepository;
        private readonly IMapper _Mapper;
        public User_TicketController(IUser_TicketRepository userTicketRepository, IMapper mapper)
        {
            _userTicketRepository = userTicketRepository;
            _Mapper = mapper;
        }

        // GET: api/UserTicket
        [HttpGet]
        public async Task<IActionResult> GetUserTickets()
        {
            var userTickets = await _userTicketRepository.GetUser_TicketAsync();
            return Ok(userTickets);
        }

        // GET: api/UserTicket/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserTicketById(Guid id)
        {
            var userTicket = await _userTicketRepository.GetUser_TicketByIdAsync(id);
            return Ok(userTicket);
        }

        // GET: api/UserTicket/{ticketId}/{userId}
        [HttpGet("{liveId}/{userId}")]
        public async Task<IActionResult> GetUserTicketByUserIdLiveId(Guid liveId, string userId)
        {
            var userTicket = await _userTicketRepository.FindUserTicketByUserIdAndLiveStreamIdAsync(liveId, userId);
            if (userTicket == null)
            {
                return Ok(null);
            }
            return Ok(userTicket);
        }

        // POST: api/UserTicket
        [HttpPost]
        public async Task<IActionResult> AddUserTicket([FromBody] User_TicketModel model)
        {
            model.Id = Guid.NewGuid();
            model.CreateDate = DateTime.Now;
            var userTicket = _Mapper.Map<User_Ticket>(model);
            var createdUserTicket = await _userTicketRepository.AddUser_TicketAsync(userTicket);
            return Ok(model);
        }
    }
}