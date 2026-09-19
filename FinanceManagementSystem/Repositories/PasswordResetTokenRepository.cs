using Dapper;
using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;

namespace FinanceManagementSystem.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public PasswordResetTokenRepository(
        DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(
        PasswordResetToken resetToken)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            """
            INSERT INTO PasswordResetTokens
            (
                UserId,
                Token,
                ExpiresAt,
                IsUsed,
                CreatedAt
            )
            VALUES
            (
                @UserId,
                @Token,
                @ExpiresAt,
                @IsUsed,
                @CreatedAt
            )
            """,
            resetToken);
    }

    public async Task<PasswordResetToken?> GetValidTokenAsync(
        string token)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<PasswordResetToken>(
            """
            SELECT
                TokenId,
                UserId,
                Token,
                ExpiresAt,
                IsUsed,
                CreatedAt
            FROM PasswordResetTokens
            WHERE Token = @Token
              AND IsUsed = 0
              AND ExpiresAt > @CurrentTime
            """,
            new
            {
                Token = token,
                CurrentTime = DateTime.UtcNow
            });
    }

    public async Task MarkAsUsedAsync(
        int tokenId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            """
            UPDATE PasswordResetTokens
            SET IsUsed = 1
            WHERE TokenId = @TokenId
            """,
            new { TokenId = tokenId });
    }
}
