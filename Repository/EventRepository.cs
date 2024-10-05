using Entity;
using Entity.Data;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _db;

        public EventRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Event>> GetEventsAsync()
        {
            return await _db.Events.ToListAsync();
        }

        public async Task<Event> GetEventsByIdAsync(int id)
        {
            return await _db.Events.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Event>> GetEventsByUserIdAsync(string userId)
        {
            return await _db.Events
                            .Where(p => p.UserId == userId)
                            .ToListAsync();
        }

        public async Task<Event> CreateEventAsync(Event Event)
        {
            await _db.Events.AddAsync(Event);
            await _db.SaveChangesAsync();
            return Event;
        }

        public async Task<Event> UpdateEventAsync(Event Event)
        {
            _db.Events.Update(Event);
            await _db.SaveChangesAsync();
            return Event;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var Event = await _db.Events.FirstOrDefaultAsync(p => p.Id == id);
            if (Event != null)
            {
                _db.Events.Remove(Event);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
