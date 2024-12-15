using Entity.Data;
using Entity.Live;
using Microsoft.EntityFrameworkCore;
using Model.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Live
{
    public class LiveStreamRepository
    {
        private readonly AppDbContext _context;

        public LiveStreamRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<LiveStream>> GetLiveStreamsAsync()
        {
            return await _context.LiveStreams.ToListAsync();
        }
        public async Task<LiveStream> GetLiveStreamBtIdAsync(Guid id)
        {
            return await _context.LiveStreams.FirstOrDefaultAsync(o=>o.Id.Equals(id));
        }
        public async Task<LiveStream> AddLiveStreamAsync(Guid id)
        {
            var LiveStream = new LiveStream
            {
                Id = id,
                Status = 1,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                UserId = id.ToString(),
            };
            _context.LiveStreams.Add(LiveStream);

            await _context.SaveChangesAsync();

            return LiveStream;

        }
        public async Task<LiveStream> UpdateLiveStreamAsync(LiveStream model)
        {

            var existLiveStream = GetLiveStreamBtIdAsync(model.Id);
            if (existLiveStream != null)
            {
                _context.Entry(existLiveStream).CurrentValues.SetValues(model);
            }
            else
            {
                _context.LiveStreams.Add(model);
            }
            await _context.SaveChangesAsync();

            return model;

        }
    }
}
