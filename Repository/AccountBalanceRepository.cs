using Entity;
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

namespace Repository
{
    public class AccountBalanceRepository : IAccountBalanceRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountBalanceRepository(AppDbContext context)
        {
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
     
        public async Task<Boolean> UpdateBalanceAsync(String UserId, int money, String signature)
        {

            var existAccountBalance =  await _context.AccountBalances.FirstOrDefaultAsync(t => t.UserId.Equals(UserId));

            string data = $"UserId={UserId}&money={money}";
            string checksumKey = _configuration["AppSettings:ChecksumKey"];

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(checksumKey));
            
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            String _signature= BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            
            if (existAccountBalance != null&& signature.Equals(_signature))
            {
                var Bala = new AccountBalance
                {
                    Id = existAccountBalance.Id,
                    Balance= existAccountBalance.Balance+money,
                    LastUpdated=DateTime.Now,
                    UserId = UserId

                };
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
