using Dapper;
using FinanceManagementSystem.Data;
using System.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;

namespace FinanceManagementSystem.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public AccountRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Account>> GetByUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT AccountId, UserId, AccountName,
                   AccountType, Balance, CreatedAt
            FROM Accounts
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC
            """;

        return await connection.QueryAsync<Account>(
            sql,
            new { UserId = userId });
    }

    public async Task<Account?> GetByIdAsync(int accountId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT AccountId, UserId, AccountName,
                   AccountType, Balance, CreatedAt
            FROM Accounts
            WHERE AccountId = @AccountId
            """;

        return await connection.QueryFirstOrDefaultAsync<Account>(
            sql,
            new { AccountId = accountId });
    }

    public async Task<Account?> GetByIdAsync(
    int accountId,
    IDbTransaction dbTransaction)
    {
        return await dbTransaction.Connection!
            .QuerySingleOrDefaultAsync<Account>(
                """
            SELECT
                AccountId,
                UserId,
                AccountName,
                AccountType,
                Balance,
                CreatedAt
            FROM Accounts
            WHERE AccountId = @AccountId
            """,
                new { AccountId = accountId },
                dbTransaction);
    }

    public async Task<int> CreateAsync(Account account)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Accounts
                (UserId, AccountName, AccountType, Balance)
            OUTPUT INSERTED.AccountId
            VALUES
                (@UserId, @AccountName, @AccountType, @Balance)
            """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            account);
    }

    public async Task UpdateBalanceAsync(
    int accountId,
    decimal newBalance,
    IDbTransaction transaction)
    {
        const string sql = """
        UPDATE Accounts
        SET Balance = @Balance
        WHERE AccountId = @AccountId
        """;

        await transaction.Connection!.ExecuteAsync(
            sql,
            new
            {
                AccountId = accountId,
                Balance = newBalance
            },
            transaction);
    }
}
