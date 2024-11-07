using Entity.Data;
using Entity.Live;
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
    }
}
