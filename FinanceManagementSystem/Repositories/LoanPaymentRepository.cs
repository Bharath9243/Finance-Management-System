using Dapper;
using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using System.Data;

namespace FinanceManagementSystem.Repositories;

public class LoanPaymentRepository : ILoanPaymentRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public LoanPaymentRepository(
        DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(
        LoanPayment payment,
        IDbTransaction dbTransaction)
    {
        return await dbTransaction.Connection!
            .ExecuteScalarAsync<int>(
                """
                INSERT INTO LoanPayments
                (
                    LoanId,
                    AccountId,
                    Amount,
                    PrincipalPaid,
                    InterestPaid,
                    PaymentDate
                )
                OUTPUT INSERTED.PaymentId
                VALUES
                (
                    @LoanId,
                    @AccountId,
                    @Amount,
                    @PrincipalPaid,
                    @InterestPaid,
                    SYSUTCDATETIME()
                )
                """,
                payment,
                dbTransaction);
    }

    public async Task<IEnumerable<LoanPayment>> GetByLoanIdAsync(
        int loanId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QueryAsync<LoanPayment>(
            """
            SELECT
                PaymentId,
                LoanId,
                AccountId,
                Amount,
                PrincipalPaid,
                InterestPaid,
                PaymentDate
            FROM LoanPayments
            WHERE LoanId = @LoanId
            ORDER BY PaymentDate DESC
            """,
            new { LoanId = loanId });
    }
}