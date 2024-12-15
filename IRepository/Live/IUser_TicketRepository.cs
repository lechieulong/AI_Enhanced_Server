using Entity.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository.Live
{
    public interface IUser_TicketRepository
    {
        Task<IEnumerable<User_Ticket>> GetUser_TicketAsync();
        Task<User_Ticket> GetUser_TicketByIdAsync(Guid Id);
        Task<User_Ticket> FindUserTicketByUserIdAndLiveStreamIdAsync(Guid liveStreamId, String UserId);
        Task<User_Ticket> AddUser_TicketAsync(User_Ticket model);
    }
}