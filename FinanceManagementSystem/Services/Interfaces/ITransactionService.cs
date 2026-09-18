using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Services.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<Transaction>> GetMyTransactionsAsync(
        int userId);
}