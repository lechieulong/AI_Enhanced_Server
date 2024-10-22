using Entity.Live;
using Model.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository.Live
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetActiveTicketsByLiveIdAsync(Guid LiveId);
        Task<Ticket> addTicketAsync(Ticket mode);
    }
}
