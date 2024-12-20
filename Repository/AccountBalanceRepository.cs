using Entity.Data;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.Xml;
using Microsoft.Extensions.Configuration;
using Entity.Payment;
using Microsoft.Extensions.Configuration;
using Entity;
using Microsoft.AspNetCore.Identity;

namespace Repository
{
    public class AccountBalanceRepository : IAccountBalanceRepository
    {
        private readonly string _checksumKey;
        private readonly AppDbContext _context;

        public AccountBalanceRepository(AppDbContext context, IConfiguration configuration)
        {
            _checksumKey = configuration["ApiSettings:ChecksumKey"];
            _context = context;
        }
        public async Task<decimal> GetBalace(string UserId)
        {
            var balace=await _context.AccountBalances.FirstOrDefaultAsync(t=>t.UserId.Equals(UserId));
            if(balace == null)
            {
                AddNew(UserId);
                return 0;
            }
            else
            {
                return balace.Balance;
            }
        }
        public async Task<AccountBalance> AddNew(string UserId)
        {
            var Balance = new AccountBalance
            {
               Id=Guid.NewGuid(),
               UserId = UserId,
               Balance=0,
               LastUpdated=DateTime.Now
            };
            _context.AccountBalances.Add(Balance);

            await _context.SaveChangesAsync();
            return Balance;
        }
     
        public async Task<Boolean> UpdateBalanceAsync(AccountBalaceModel model)
        {

            var existAccountBalance =  await _context.AccountBalances.FirstOrDefaultAsync(t => t.UserId.Equals(model.UserId));

            string data = $"userid={model.UserId}&money={model.Balance}&message={model.Message}&type={model.Type}";

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey));

            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            String _signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            if (existAccountBalance != null && _signature.Equals(model.signature))
            {
                var Bala = new AccountBalance
                {
                    Id = existAccountBalance.Id,
                    Balance= existAccountBalance.Balance+model.Balance,
                    LastUpdated=DateTime.Now,
                    UserId = model.UserId

                };

                var history = new Balance_History
                {
                    Id = Guid.NewGuid(),
                    AccountBalanceId = existAccountBalance.Id,
                    amount = model.Balance,
                    Description = model.Message,
                    Type=model.Type,
                    CreateDate = DateTime.Now,

                };
                _context.Balance_Historys.Add(history);
                _context.Entry(existAccountBalance).CurrentValues.SetValues(Bala);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }           

        }
        public async Task<IEnumerable<Balance_History>> GetBalanceHistoryByUserIdAsync(string userId)
        {
            return await _context.Balance_Historys
                .Include(bh => bh.AccountBalance)
                .Where(bh => bh.AccountBalance != null && bh.AccountBalance.UserId == userId)
                .OrderByDescending(bh => bh.CreateDate)
                .ToListAsync();
        }

        public async Task<Boolean> UpdateBalance(AccountBalaceModel model)
        {

            var existAccountBalance = await _context.AccountBalances.FirstOrDefaultAsync(t => t.UserId.Equals(model.UserId));




            if (existAccountBalance != null )
            {
                var Bala = new AccountBalance
                {
                    Id = existAccountBalance.Id,
                    Balance = existAccountBalance.Balance + model.Balance,
                    LastUpdated = DateTime.Now,
                    UserId = model.UserId

                };

                var history = new Balance_History
                {
                    Id = Guid.NewGuid(),
                    AccountBalanceId = existAccountBalance.Id,
                    amount = model.Balance,
                    Description = model.Message,
                    Type = model.Type,
                    CreateDate = DateTime.Now,

                };
                _context.Balance_Historys.Add(history);
                _context.Entry(existAccountBalance).CurrentValues.SetValues(Bala);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
