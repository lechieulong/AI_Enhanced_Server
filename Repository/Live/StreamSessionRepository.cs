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
    public class StreamSessionRepository:IStreamSessionRepository
    {
        private readonly AppDbContext _context;

        public StreamSessionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<StreamSession>> getStreamSessionsIsLive()
        {
            return await _context.StreamSessions.Where(o=>o.Status==1).ToListAsync();

        }
        public async Task<StreamSession> AddStreamSessionAsync(StreamSession mode)
        {
            _context.StreamSessions.Add(mode);

            await _context.SaveChangesAsync();

            return mode;
        }
        public async Task<StreamSession> UpdateStreamSessionAsync(StreamSession mode)
        {
            var exit=await _context.StreamSessions.FirstOrDefaultAsync(o=>o.Id.Equals(mode.Id));
            StreamSession modee = new StreamSession()
            {
                Id = mode.Id,
                Status = mode.Status,
                StartTime = mode.StartTime,
                EndTime = mode.EndTime,
                LiveStreamId = mode.LiveStreamId,

            };
            if (exit != null)
            {
                _context.Entry(exit).CurrentValues.SetValues(modee);
            }
            else
            {
                _context.StreamSessions.Add(mode);
            }
            

            await _context.SaveChangesAsync();

            return mode;
        }

    }
}
