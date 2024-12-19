using Entity;
using Entity.Data;
using Entity.Live;
using IRepository.Live;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Model.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Live
{
    public class LiveStreamRepository: ILiveStreamRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LiveStreamRepository(AppDbContext context, UserManager<ApplicationUser> user)
        {
            _context = context;
            _userManager = user;
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

            var existLiveStream = await GetLiveStreamBtIdAsync(model.Id);
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
        public async Task<(IEnumerable<LiveStream> lives, int totalCount)> GetLivesBlockAsync(int page, int pageSize, string? searchQuery)
        {

            var totalCount = await _context.LiveStreams.CountAsync();
            var live = new List<LiveStream>();

            live = await _context.LiveStreams.Include(o=>o.User).Include(o=>o.StreamSessions.Where(c=>c.Status==1)).OrderBy(o=>o.Status).Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
            foreach (var liveStream in live.ToList()) 
            {
                var user = await _userManager.FindByIdAsync(liveStream.User.Id);
                if(user != null)
                {
                   if (await _userManager.IsInRoleAsync(user, "ADMIN"))
                    {
                        live.Remove(liveStream);
                    }
                }             
            }
            
            return (live, totalCount);
        }
    }
}
