using Dapper;
using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using System.Data;

namespace FinanceManagementSystem.Repositories;

public class LoanProductRepository : ILoanProductRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public LoanProductRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<LoanProduct>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                LoanProductId,
                Name,
                Description,
                InterestRate,
                InterestType,
                MaxAmount,
                MaxTenureMonths,
                IsActive,
                CreatedAt
            FROM LoanProducts
            ORDER BY CreatedAt DESC;
            """;

        return await connection.QueryAsync<LoanProduct>(sql);
    }

    public async Task<IEnumerable<LoanProduct>> GetActiveAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                LoanProductId,
                Name,
                Description,
                InterestRate,
                InterestType,
                MaxAmount,
                MaxTenureMonths,
                IsActive,
                CreatedAt
            FROM LoanProducts
            WHERE IsActive = 1
            ORDER BY Name;
            """;

        return await connection.QueryAsync<LoanProduct>(sql);
    }

    public async Task<LoanProduct?> GetByIdAsync(int loanProductId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                LoanProductId,
                Name,
                Description,
                InterestRate,
                InterestType,
                MaxAmount,
                MaxTenureMonths,
                IsActive,
                CreatedAt
            FROM LoanProducts
            WHERE LoanProductId = @LoanProductId;
            """;

        return await connection.QuerySingleOrDefaultAsync<LoanProduct>(
            sql,
            new { LoanProductId = loanProductId });
    }

    public async Task<int> CreateAsync(
    LoanProduct product,
    IDbTransaction dbTransaction)
    {
        return await dbTransaction.Connection!
            .ExecuteScalarAsync<int>(
                """
            INSERT INTO LoanProducts
            (
                Name,
                Description,
                InterestRate,
                InterestType,
                MaxAmount,
                MaxTenureMonths,
                IsActive,
                CreatedAt
            )
            VALUES
            (
                @Name,
                @Description,
                @InterestRate,
                @InterestType,
                @MaxAmount,
                @MaxTenureMonths,
                @IsActive,
                @CreatedAt
            );

            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """,
                product,
                dbTransaction);
    }

    public async Task<bool> UpdateAsync(
    LoanProduct product,
    IDbTransaction dbTransaction)
    {
        var rowsAffected =
            await dbTransaction.Connection!
                .ExecuteAsync(
                    """
                UPDATE LoanProducts
                SET
                    Name = @Name,
                    Description = @Description,
                    InterestRate = @InterestRate,
                    InterestType = @InterestType,
                    MaxAmount = @MaxAmount,
                    MaxTenureMonths = @MaxTenureMonths,
                    IsActive = @IsActive
                WHERE LoanProductId = @LoanProductId;
                """,
                    product,
                    dbTransaction);

        return rowsAffected > 0;
    }
}
