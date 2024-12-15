using Entity.Data;
using Entity.Live;
using Model.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository.Live
{
    public interface IGiftRepository
    {
        Task<IEnumerable<Gift>> GetGiftAsync();
        Task<Gift> GetGiftByIDAsync(Guid id);
        Task<Gift> AddGiftAsync(Gift model);
        Task<Gift> UpdateGiftAsync(Gift model);
        Task<Gift> DeleteAsync(Guid id);
        Task<(IEnumerable<Gift> users, int totalCount)> GetUsersAsync(int page, int pageSize,String? searchQuery);
    }
}
