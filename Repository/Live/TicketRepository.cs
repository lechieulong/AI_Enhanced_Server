using Entity.Data;
using Entity.Live;
using IRepository.Live;
using Microsoft.EntityFrameworkCore;
using Model.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Live
{
    public class TicketRepository:ITicketRepository
    {

        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Ticket>> GetActiveTicketsByLiveIdAsync(Guid liveId)
        {
            var currentTime = DateTime.UtcNow;
            return await _context.Tickets
                .Where(o => o.LiveStreamId.Equals(liveId) && o.StartTime < currentTime && currentTime < o.EndTime)
                .ToListAsync();
        }
        public async Task<Ticket> addTicketAsync(Ticket mode)
        {
            _context.Tickets.Add(mode);
            await _context.SaveChangesAsync();
            return mode;

        }
    }
}
