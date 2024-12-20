using Entity.Payment;
using Model;
using Model.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IAccountBalanceRepository
    {
        Task<decimal> GetBalace(string UserId);
        Task<AccountBalance> AddNew(String UserId);

        Task<Boolean> UpdateBalanceAsync(AccountBalaceModel mode);

        Task<IEnumerable<Balance_History>> GetBalanceHistoryByUserIdAsync(string userId);
        Task<Boolean> UpdateBalance(AccountBalaceModel model);
    }
}
