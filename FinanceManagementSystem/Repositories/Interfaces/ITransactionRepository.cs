using System.Data;
using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Repositories.Interfaces;

public interface ITransactionRepository
{
    Task<int> CreateAsync(
        Transaction transaction,
        IDbTransaction dbTransaction);

    Task<IEnumerable<Transaction>> GetByAccountIdsAsync(
    IEnumerable<int> accountIds);
}