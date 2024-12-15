using Entity.Data;
using Entity.Live;
using IRepository.Live;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Live
{
    public class User_TicketRepository : IUser_TicketRepository
    {
        private readonly AppDbContext _context;

        public User_TicketRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User_Ticket>> GetUser_TicketAsync()
        {
            return await _context.User_Tickets.ToListAsync();
        }
        public async Task<User_Ticket> GetUser_TicketByIdAsync(Guid Id)
        {
            return await _context.User_Tickets.FirstOrDefaultAsync(o => o.Id.Equals(Id));
        }

        public async Task<User_Ticket?> FindUserTicketByUserIdAndLiveStreamIdAsync(Guid liveStreamId, String UserId)
        {
            var currentTime = DateTime.Now;
            return await _context.User_Tickets
        .Include(ut => ut.Ticket)
        .Where(ut => ut.UserId == UserId && ut.Ticket.LiveStreamId == liveStreamId && ut.Ticket.StartTime <= currentTime && ut.Ticket.EndTime > currentTime)
        .FirstOrDefaultAsync();
        }
        public async Task<User_Ticket> AddUser_TicketAsync(User_Ticket model)
        {
            _context.User_Tickets.Add(model);
            await _context.SaveChangesAsync();
            return model;

        }
    }
}