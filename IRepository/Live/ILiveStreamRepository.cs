using Entity.Data;
using Entity.Live;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository.Live
{
    public interface ILiveStreamRepository
    {
        Task<IEnumerable<LiveStream>> GetLiveStreamsAsync();
        Task<LiveStream> GetLiveStreamBtIdAsync(Guid id);
        Task<LiveStream> AddLiveStreamAsync(Guid id);

        Task<LiveStream> UpdateLiveStreamAsync(LiveStream model);
        Task<(IEnumerable<LiveStream> lives, int totalCount)> GetLivesBlockAsync(int page, int pageSize, string? searchQuery);
    }
}
