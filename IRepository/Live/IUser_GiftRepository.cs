using Entity.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository.Live
{
    public interface IUser_GiftRepository
    {
        Task<IEnumerable<User_Gift>> GetUser_GiftAsync();
        Task<User_Gift> GetUser_GiftByIDAsync(Guid id);
        Task<IEnumerable<User_Gift>> GetUser_GiftByUserId(string id);
        Task<IEnumerable<User_Gift>> GetUser_GiftByReceiverId(string id);
        Task<User_Gift> AddUser_GiftAsync(User_Gift model);
        Task<User_Gift> UpdateGiftAsync(User_Gift model);
    }
}