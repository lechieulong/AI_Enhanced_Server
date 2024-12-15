using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Data;
using Entity.Payment;
using Entity.Test;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Model.Payment;
using Model.Test;

namespace Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<TransactionModel> AddTransactionAsync(TransactionModel model)
        {
            var Transaction = new Transaction
            {
                Id = 0,
                Amount = model.Amount,
                CreatedAt = DateTime.Now,
                PaymentStatus = model.PaymentStatus,
                UserId = model.UserId,
            };
            _context.Transactions.Add(Transaction);
       
            await _context.SaveChangesAsync();

            model.Id = Transaction.Id;
            return model;

        }
        public async Task<TransactionModel> UpdateTransactionAsync(TransactionModel model)
        {

            var existTransaction= await GetTransactionByIdAsync(model.Id);
            var Transaction = new Transaction
            {
                Id = model.Id,
                Amount = model.Amount,
                CreatedAt = DateTime.Now,
                PaymentStatus = model.PaymentStatus,
                UserId = model.UserId,
            };
            if (existTransaction != null)
            {
                _context.Entry(existTransaction).CurrentValues.SetValues(Transaction);
            }
            else
            {
                _context.Transactions.Add(Transaction);
            }         
            await _context.SaveChangesAsync();

            model.Id = Transaction.Id;
            return model;

        }
        public async Task<(IEnumerable<Transaction> transactions, int totalCount)> GetTransactionAsyn(int page, int pageSize, string? searchQuery)
        {

            var totalCount = await _context.Transactions.CountAsync();
            var transactions = new List<Transaction>();
            if (searchQuery != null)
            {
                transactions = await _context.Transactions.Include(u => u.User).OrderBy(u => u.CreatedAt).Where(u => u.User.Name.Contains(searchQuery)).Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
            }
            else
            {
                transactions = await _context.Transactions.Include(u => u.User).OrderBy(u => u.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();

            }



            return (transactions, totalCount);
        }
    }
}
