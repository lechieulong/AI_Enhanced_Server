using Entity;
using Model.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Payment;

namespace IRepository
{
    public interface ITransactionRepository
    {
        Task<TransactionMode> AddTransactionAsync(TransactionMode model);
        Task<Transaction> GetTransactionByIdAsync(int id);
        Task<TransactionMode> UpdateTransactionAsync(TransactionMode model);
    }
}
