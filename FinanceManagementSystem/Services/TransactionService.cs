using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class TransactionService : ITransactionService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public TransactionService(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<Transaction>> GetMyTransactionsAsync(
        int userId)
    {
        var accounts =
            await _accountRepository.GetByUserIdAsync(userId);

        var accountIds =
            accounts.Select(a => a.AccountId).ToList();

        if (!accountIds.Any())
        {
            return Enumerable.Empty<Transaction>();
        }

        return await _transactionRepository
            .GetByAccountIdsAsync(accountIds);
    }
}


