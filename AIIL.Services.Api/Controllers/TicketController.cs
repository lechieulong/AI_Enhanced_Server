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


        public TicketController(ITicketRepository repository, IMapper Mapper)
        {
            _repository = repository;
            _Mapper = Mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketByLiveId(Guid id)
        {
            var Tikcet = await _repository.GetActiveTicketsByLiveIdAsync(id);
            return Ok(Tikcet);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] TicketModel ticketDto)
        {
            var ticketexit = await _repository.GetActiveTicketsByLiveIdAsync(ticketDto.LiveStreamId);
            if (ticketDto == null)
            {
                return BadRequest("Invalid data.");
            }

            var ticket = _Mapper.Map<Ticket>(ticketDto);

            if (ticketexit == null|| ticketexit.Price != ticketDto.Price|| ticketexit.StartTime!= ticketDto.StartTime|| ticketexit.EndTime!= ticketDto.EndTime) {         
            ticket.Id=Guid.NewGuid();
            ticket.CreateDate = DateTime.Now;
            var result = await _repository.addTicketAsync(ticket);
            return Ok(result);
            }
            else
            {
                ticket.CreateDate = DateTime.Now;
                var result = await _repository.UpdateTicketAsync(ticket);
                return Ok(result);
            }
            

            
        }

    }
}