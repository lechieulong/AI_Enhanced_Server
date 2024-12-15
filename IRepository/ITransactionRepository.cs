using Model.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Payment;
using Entity.Payment;

namespace IRepository
{
    public interface ITransactionRepository
    {
        Task<TransactionModel> AddTransactionAsync(TransactionModel model);
        Task<Transaction> GetTransactionByIdAsync(int id);
        Task<TransactionModel> UpdateTransactionAsync(TransactionModel model);
        Task<(IEnumerable<Transaction> transactions, int totalCount)> GetTransactionAsyn(int page, int pageSize, string? searchQuery);
    }
}
