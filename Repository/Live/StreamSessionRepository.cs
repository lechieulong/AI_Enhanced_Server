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
        public async Task<StreamSession> getStreamSession(Guid liveId)
        {
            return await _context.StreamSessions.FirstOrDefaultAsync(o => o.LiveStreamId.Equals(liveId)&&o.Status==1);

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
            var exit=await getStreamSession(mode.LiveStreamId);
            StreamSession modee = new StreamSession()
            {
                Id = exit.Id,
                Status = mode.Status,
                Name=mode.Name,
                StartTime = mode.StartTime,
                EndTime = mode.Status==0?DateTime.Now:null,
                LiveStreamId = mode.LiveStreamId,
                Type = mode.Type,

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
