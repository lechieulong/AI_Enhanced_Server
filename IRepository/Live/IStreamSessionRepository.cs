using Entity.Live;
using Microsoft.EntityFrameworkCore;
using Model.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository.Live
{
    public interface IStreamSessionRepository
    {
        Task<StreamSession>  getStreamSession(Guid id);
        Task<IEnumerable<StreamSession>> getStreamSessionsIsLive();
        Task<StreamSession> AddStreamSessionAsync(StreamSession mode);
        Task<StreamSession> UpdateStreamSessionAsync(StreamSession mode);
        Task<(IEnumerable<StreamSession> lives, int totalCount)> GetLivesAsync(int page, int pageSize, string? searchQuery);
    }
}
