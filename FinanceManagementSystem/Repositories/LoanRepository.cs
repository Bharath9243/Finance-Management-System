using Dapper;
using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using System.Data;

namespace FinanceManagementSystem.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public LoanRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(
        Loan loan,
        IDbTransaction dbTransaction)
    {
        return await dbTransaction.Connection!
            .ExecuteScalarAsync<int>(
                """
                INSERT INTO Loans
                (
                    UserId,
                    AccountId,
                    LoanProductId,
                    PrincipalAmount,
                    InterestRate,
                    InterestType,
                    TenureMonths,
                    TotalInterest,
                    TotalPayable,
                    OutstandingAmount,
                    Status
                )
                OUTPUT INSERTED.LoanId
                VALUES
                (
                    @UserId,
                    @AccountId,
                    @LoanProductId,
                    @PrincipalAmount,
                    @InterestRate,
                    @InterestType,
                    @TenureMonths,
                    @TotalInterest,
                    @TotalPayable,
                    @OutstandingAmount,
                    @Status
                )
                """,
                loan,
                dbTransaction);
    }

    public async Task<Loan?> GetByIdAsync(int loanId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Loan>(
            """
            SELECT
                LoanId,
                UserId,
                AccountId,
                LoanProductId,
                PrincipalAmount,
                InterestRate,
                InterestType,
                TenureMonths,
                TotalInterest,
                TotalPayable,
                OutstandingAmount,
                Status,
                StartDate,
                CreatedAt,
                ApprovedAt
            FROM Loans
            WHERE LoanId = @LoanId
            """,
            new { LoanId = loanId });
    }

    public async Task<IEnumerable<Loan>> GetByUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<Loan>(
            """
            SELECT
                LoanId,
                UserId,
                AccountId,
                LoanProductId,
                PrincipalAmount,
                InterestRate,
                InterestType,
                TenureMonths,
                TotalInterest,
                TotalPayable,
                OutstandingAmount,
                Status,
                StartDate,
                CreatedAt,
                ApprovedAt
            FROM Loans
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC
            """,
            new { UserId = userId });
    }

    public async Task<IEnumerable<Loan>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<Loan>(
            """
            SELECT
                LoanId,
                UserId,
                AccountId,
                LoanProductId,
                PrincipalAmount,
                InterestRate,
                InterestType,
                TenureMonths,
                TotalInterest,
                TotalPayable,
                OutstandingAmount,
                Status,
                StartDate,
                CreatedAt,
                ApprovedAt
            FROM Loans
            ORDER BY CreatedAt DESC
            """);
    }

    public async Task UpdateStatusAsync(
        int loanId,
        string status,
        DateTime? approvedAt,
        DateTime? startDate,
        IDbTransaction dbTransaction)
    {
        await dbTransaction.Connection!
            .ExecuteAsync(
                """
                UPDATE Loans
                SET
                    Status = @Status,
                    ApprovedAt = @ApprovedAt,
                    StartDate = @StartDate
                WHERE LoanId = @LoanId
                """,
                new
                {
                    LoanId = loanId,
                    Status = status,
                    ApprovedAt = approvedAt,
                    StartDate = startDate
                },
                dbTransaction);
    }

    public async Task UpdateOutstandingAmountAsync(
        int loanId,
        decimal outstandingAmount,
        string status,
        IDbTransaction dbTransaction)
    {
        await dbTransaction.Connection!
            .ExecuteAsync(
                """
                UPDATE Loans
                SET
                    OutstandingAmount = @OutstandingAmount,
                    Status = @Status
                WHERE LoanId = @LoanId
                """,
                new
                {
                    LoanId = loanId,
                    OutstandingAmount = outstandingAmount,
                    Status = status
                },
                dbTransaction);
    }
}
