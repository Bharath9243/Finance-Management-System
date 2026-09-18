using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Services.Interfaces;

public interface IAccountService
{
    Task<IEnumerable<Account>> GetMyAccountsAsync(int userId);

    Task<Account?> GetAccountAsync(int userId, int accountId);

    Task<int> CreateAccountAsync(
        int userId,
        string accountName,
        string accountType);

    Task DepositAsync(
    int userId,
    int accountId,
    decimal amount,
    string? description);

    Task WithdrawAsync(
    int userId,
    int accountId,
    decimal amount,
    string? description);

    Task AddIncomeAsync(
    int userId,
    int accountId,
    decimal amount,
    int? categoryId,
    string? description);

    Task AddExpenseAsync(
        int userId,
        int accountId,
        decimal amount,
        int? categoryId,
        string? description);

    Task TransferAsync(
    int userId,
    int fromAccountId,
    int toAccountId,
    decimal amount,
    string? description);
}