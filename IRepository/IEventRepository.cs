using Entity;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetEventsAsync();
        Task<Event> GetEventsByIdAsync(Guid id);
        Task<IEnumerable<Event>> GetEventsByUserIdAsync(string userId);
        Task<Event> CreateEventAsync(Event Event);
        Task<Event> UpdateEventAsync(Event Event);
        Task<bool> DeleteEventAsync(Guid id);
    }
}
