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
    public class TicketRepository : ITicketRepository
    {

        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Ticket> GetActiveTicketsByLiveIdAsync(Guid liveId)
        {
            var currentTime = DateTime.Now;
            return await _context.Tickets
                .Where(o => o.LiveStreamId.Equals(liveId) && o.StartTime <= currentTime && currentTime < o.EndTime)
                .OrderByDescending(o => o.CreateDate) // Sắp xếp theo CreateDate giảm dần
                .FirstOrDefaultAsync(); // Lấy bản ghi đầu tiên (mới nhất) hoặc null nếu không có
        }
        public async Task<Ticket> addTicketAsync(Ticket mode)
        {
            _context.Tickets.Add(mode);
            await _context.SaveChangesAsync();
            return mode;

        }
        public async Task<Ticket> UpdateTicketAsync(Ticket model)
        {
            var existTicket = await GetActiveTicketsByLiveIdAsync(model.LiveStreamId);
            if (existTicket != null)
            {
                _context.Entry(existTicket).CurrentValues.SetValues(model);
            }
            else
            {
                _context.Tickets.Add(model);
            }
            await _context.SaveChangesAsync();

            return model;
        }
    }
}