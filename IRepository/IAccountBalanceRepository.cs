using Entity;
using Model;
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

        Task<Boolean> UpdateBalanceAsync(String UserId, int money,String signature);

    }
}
