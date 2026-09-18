using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories;
using Microsoft.Data.SqlClient;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly DbConnectionFactory _connectionFactory;
    private readonly ITransferRepository _transferRepository;

    public AccountService(
    DbConnectionFactory connectionFactory,
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    ITransferRepository transferRepository)
    {
        _connectionFactory = connectionFactory;
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _transferRepository = transferRepository;
    }

    public async Task<IEnumerable<Account>> GetMyAccountsAsync(int userId)
    {
        return await _accountRepository.GetByUserIdAsync(userId);
    }

    public async Task<Account?> GetAccountAsync(
        int userId,
        int accountId)
    {
        var account = await _accountRepository.GetByIdAsync(accountId);

        if (account == null)
        {
            return null;
        }

        if (account.UserId != userId)
        {
            return null;
        }

        return account;
    }

    public async Task<int> CreateAccountAsync(
    int userId,
    string accountName,
    string accountType)
    {
        if (string.IsNullOrWhiteSpace(accountName))
        {
            throw new ArgumentException("Account name is required.");
        }

        var validAccountTypes = new[]
        {
        "Savings",
        "Checking",
        "Cash"
    };

        if (!validAccountTypes.Contains(accountType))
        {
            throw new ArgumentException("Invalid account type.");
        }

        accountName = accountName.Trim();

        // Check whether the user already has an account
        // with the same name.
        var existingAccounts =
            await _accountRepository.GetByUserIdAsync(userId);

        if (existingAccounts.Any(a =>
            string.Equals(
                a.AccountName,
                accountName,
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException(
                "An account with this name already exists.");
        }

        var account = new Account
        {
            UserId = userId,
            AccountName = accountName,
            AccountType = accountType,
            Balance = 0
        };

        try
        {
            return await _accountRepository.CreateAsync(account);
        }
        catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
        {
            // Protect against a duplicate created concurrently.
            throw new ArgumentException(
                "An account with this name already exists.");
        }
    }

    public async Task DepositAsync(
    int userId,
    int accountId,
    decimal amount,
    string? description)
    {
        if (amount <= 0)
        {
            throw new ArgumentException(
                "Deposit amount must be greater than zero.");
        }

        var account = await _accountRepository.GetByIdAsync(accountId);

        if (account == null)
        {
            throw new ArgumentException("Account not found.");
        }

        if (account.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You cannot access this account.");
        }

        var newBalance = account.Balance + amount;

        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var dbTransaction = connection.BeginTransaction();

        try
        {
            await _accountRepository.UpdateBalanceAsync(
                accountId,
                newBalance,
                dbTransaction);

            var transaction = new Transaction
            {
                AccountId = accountId,
                TransactionType = "Deposit",
                Amount = amount,
                Description = description
            };

            await _transactionRepository.CreateAsync(
                transaction,
                dbTransaction);

            dbTransaction.Commit();
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
    }

    public async Task WithdrawAsync(
    int userId,
    int accountId,
    decimal amount,
    string? description)
    {
        if (amount <= 0)
        {
            throw new ArgumentException(
                "Withdrawal amount must be greater than zero.");
        }

        var account = await _accountRepository.GetByIdAsync(accountId);

        if (account == null)
        {
            throw new ArgumentException("Account not found.");
        }

        if (account.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You cannot access this account.");
        }

        if (account.Balance < amount)
        {
            throw new ArgumentException(
                "Insufficient balance.");
        }

        var newBalance = account.Balance - amount;

        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var dbTransaction = connection.BeginTransaction();

        try
        {
            await _accountRepository.UpdateBalanceAsync(
                accountId,
                newBalance,
                dbTransaction);

            var transaction = new Transaction
            {
                AccountId = accountId,
                TransactionType = "Withdrawal",
                Amount = amount,
                Description = description
            };

            await _transactionRepository.CreateAsync(
                transaction,
                dbTransaction);

            dbTransaction.Commit();
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
    }

    public async Task AddIncomeAsync(
    int userId,
    int accountId,
    decimal amount,
    int? categoryId,
    string? description)
    {
        if (amount <= 0)
        {
            throw new ArgumentException(
                "Income amount must be greater than zero.");
        }

        var account = await _accountRepository.GetByIdAsync(accountId);

        if (account == null)
        {
            throw new ArgumentException("Account not found.");
        }

        if (account.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You cannot access this account.");
        }

        var newBalance = account.Balance + amount;

        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var dbTransaction = connection.BeginTransaction();

        try
        {
            await _accountRepository.UpdateBalanceAsync(
                accountId,
                newBalance,
                dbTransaction);

            var transaction = new Transaction
            {
                AccountId = accountId,
                CategoryId = categoryId,
                TransactionType = "Income",
                Amount = amount,
                Description = description
            };

            await _transactionRepository.CreateAsync(
                transaction,
                dbTransaction);

            dbTransaction.Commit();
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
    }

    public async Task AddExpenseAsync(
    int userId,
    int accountId,
    decimal amount,
    int? categoryId,
    string? description)
    {
        if (amount <= 0)
        {
            throw new ArgumentException(
                "Expense amount must be greater than zero.");
        }

        var account = await _accountRepository.GetByIdAsync(accountId);

        if (account == null)
        {
            throw new ArgumentException("Account not found.");
        }

        if (account.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You cannot access this account.");
        }

        if (account.Balance < amount)
        {
            throw new ArgumentException(
                "Insufficient balance.");
        }

        var newBalance = account.Balance - amount;

        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var dbTransaction = connection.BeginTransaction();

        try
        {
            await _accountRepository.UpdateBalanceAsync(
                accountId,
                newBalance,
                dbTransaction);

            var transaction = new Transaction
            {
                AccountId = accountId,
                CategoryId = categoryId,
                TransactionType = "Expense",
                Amount = amount,
                Description = description
            };

            await _transactionRepository.CreateAsync(
                transaction,
                dbTransaction);

            dbTransaction.Commit();
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
    }

    public async Task TransferAsync(
    int userId,
    int fromAccountId,
    int toAccountId,
    decimal amount,
    string? description)
    {
        if (amount <= 0)
            throw new ArgumentException("Transfer amount must be greater than zero.");

        if (fromAccountId == toAccountId)
            throw new ArgumentException(
                "Source and destination accounts must be different.");

        // Get both accounts and verify ownership
        var fromAccount =
            await _accountRepository.GetByIdAsync(fromAccountId);

        var toAccount =
            await _accountRepository.GetByIdAsync(toAccountId);

        if (fromAccount == null || toAccount == null)
            throw new InvalidOperationException("Account not found.");

        if (fromAccount.UserId != userId ||
            toAccount.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You can only transfer between your own accounts.");
        }

        if (fromAccount.Balance < amount)
            throw new InvalidOperationException(
                "Insufficient balance in the source account.");

        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var dbTransaction = connection.BeginTransaction();

        try
        {
            // 1. Debit source account
            await _accountRepository.UpdateBalanceAsync(
                fromAccountId,
                fromAccount.Balance - amount,
                dbTransaction);

            // 2. Credit destination account
            await _accountRepository.UpdateBalanceAsync(
                toAccountId,
                toAccount.Balance + amount,
                dbTransaction);

            // 3. Create transfer record
            var transferId =
                await _transferRepository.CreateAsync(
                    new Transfer
                    {
                        FromAccountId = fromAccountId,
                        ToAccountId = toAccountId,
                        Amount = amount
                    },
                    dbTransaction);

            // 4. Create source transaction
            await _transactionRepository.CreateAsync(
                new Transaction
                {
                    AccountId = fromAccountId,
                    TransferId = transferId,
                    TransactionType = "Transfer",
                    Amount = amount,
                    Description = description
                        ?? $"Transfer to {toAccount.AccountName}"
                },
                dbTransaction);

            // 5. Create destination transaction
            await _transactionRepository.CreateAsync(
                new Transaction
                {
                    AccountId = toAccountId,
                    TransferId = transferId,
                    TransactionType = "Transfer",
                    Amount = amount,
                    Description = description
                        ?? $"Transfer from {fromAccount.AccountName}"
                },
                dbTransaction);

            // 6. Everything succeeded
            dbTransaction.Commit();
        }
        catch
        {
            // Something failed → undo all database changes
            dbTransaction.Rollback();
            throw;
        }
    }
}
