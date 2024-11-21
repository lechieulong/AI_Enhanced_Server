using IRepository;
using Microsoft.AspNetCore.Mvc;
using Entity;
using System.Linq;
using System.Threading.Tasks;
using IRepository.Live;
using Model.Live;
using AutoMapper;
using Entity.Live;

namespace AIIL.Services.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _repository;
        private readonly IMapper _Mapper;


        //public TicketController(ITicketRepository repository, IMapper Mapper)
        //{
        //    _repository = repository;
        //    _Mapper = Mapper;
        //}

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetTicketByLiveId(Guid id)
        //{
        //    var Tikcet = await _repository.GetActiveTicketsByLiveIdAsync(id);
        //    return Ok(Tikcet);
        //}
        //[HttpPost]
        //public async Task<IActionResult> CreateTicket([FromBody] TicketModel ticketDto)
        //{
        //    if (ticketDto == null)
        //    {
        //        return BadRequest("Invalid data.");
        //    }
        //    var ticket = _Mapper.Map<Ticket>(ticketDto);
        //    ticket.Id = Guid.NewGuid();

        //    var result = await _repository.addTicketAsync(ticket);
        //    return Ok(ticketDto);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketByLiveId(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Live ID.");

            var tickets = await _repository.GetActiveTicketsByLiveIdAsync(id);

            if (tickets == null)
                return NotFound("No tickets found for the specified Live ID.");

            return Ok(tickets);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] TicketModel ticketDto)
        {
            if (ticketDto == null)
                return BadRequest("Invalid ticket data.");

            var ticket = _Mapper.Map<Ticket>(ticketDto);
            ticket.Id = Guid.NewGuid();

            await _repository.addTicketAsync(ticket);

            return Ok("Ticket created successfully.");
        }
    }
}