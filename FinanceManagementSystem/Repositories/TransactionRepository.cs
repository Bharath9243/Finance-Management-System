using Dapper;
using FinanceManagementSystem.Data;
using System.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;

namespace FinanceManagementSystem.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public TransactionRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(
    Transaction transaction,
    IDbTransaction dbTransaction)
    {
        const string sql = """
        INSERT INTO Transactions
            (AccountId,
             CategoryId,
             TransferId,
             LoanId,
             LoanPaymentId,
             TransactionType,
             Amount,
             Description)
        OUTPUT INSERTED.TransactionId
        VALUES
            (@AccountId,
             @CategoryId,
             @TransferId,
             @LoanId,
             @LoanPaymentId,
             @TransactionType,
             @Amount,
             @Description)
        """;

        return await dbTransaction.Connection!.ExecuteScalarAsync<int>(
            sql,
            transaction,
            dbTransaction);
    }

    public async Task<IEnumerable<Transaction>> GetByAccountIdsAsync(
    IEnumerable<int> accountIds)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
        SELECT
            t.TransactionId,
            t.AccountId,
            t.CategoryId,
            t.TransferId,
            t.LoanId,
            t.LoanPaymentId,
            t.TransactionType,
            t.Amount,
            t.Description,
            t.TransactionDate,
            c.Name AS CategoryName
        FROM Transactions t
        LEFT JOIN Categories c
            ON t.CategoryId = c.CategoryId
        WHERE t.AccountId IN @AccountIds
        ORDER BY t.TransactionDate DESC
        """;

        return await connection.QueryAsync<Transaction>(
            sql,
            new { AccountIds = accountIds });
    }
}
