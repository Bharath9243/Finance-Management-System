using FinanceManagementSystem.Models;
using System.Data;

namespace FinanceManagementSystem.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetByUserIdAsync(int userId);

    Task<Account?> GetByIdAsync(int accountId);

    Task<int> CreateAsync(Account account);

    Task UpdateBalanceAsync(
    int accountId,
    decimal newBalance,
    IDbTransaction transaction);

    Task<Account?> GetByIdAsync(
    int accountId,
    IDbTransaction dbTransaction);
}