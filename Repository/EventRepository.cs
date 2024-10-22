using Entity;
using Entity.Data;
using IRepository;
using Microsoft.AspNetCore.Identity;
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

        public async Task<Event> GetEventsByIdAsync(Guid id)
        {
            return await _db.Events.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Event>> GetEventsByUserIdAsync(string userId)
        {
            return await _db.Users
                            .Where(p => p.Id == userId)
                            .SelectMany(u => u.Events)
                            .ToListAsync();
        }

        public async Task<Event> CreateEventAsync(Event eventEntity, List<string> userIds)
        {
            await _db.Events.AddAsync(eventEntity);
            await _db.SaveChangesAsync();

            if (userIds != null && userIds.Count > 0)
            {
                var users = await _db.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

                eventEntity.Users = users;
                await _db.SaveChangesAsync();
            }

            return eventEntity;
        }

        public async Task<Event> UpdateEventAsync(Event Event)
        {
            _db.Events.Update(Event);
            await _db.SaveChangesAsync();
            return Event;
        }

        public async Task<bool> DeleteEventAsync(Guid id)
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

        public async Task<bool> AssignUser(string userId, Guid eventId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }
            var eventEntity = await _db.Events.FindAsync(eventId);
            if (eventEntity == null)
            {
                return false;
            }
            if (user.Events.Any(e => e.Id == eventId))
            {
                return false;
            }
            user.Events.Add(eventEntity);
            await _db.SaveChangesAsync();

            return true;
        }

    }
}
