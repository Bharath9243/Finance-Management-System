using Dapper;
using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using System.Data;

namespace FinanceManagementSystem.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public NotificationRepository(
        DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(
    Notification notification,
    IDbTransaction dbTransaction)
    {
        return await dbTransaction.Connection!
            .ExecuteScalarAsync<int>(
                """
            INSERT INTO Notifications
            (
                UserId,
                Title,
                Message,
                NotificationType
            )
            OUTPUT INSERTED.NotificationId
            VALUES
            (
                @UserId,
                @Title,
                @Message,
                @NotificationType
            )
            """,
                notification,
                dbTransaction);
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(
        int userId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        return await connection.QueryAsync<Notification>(
            """
            SELECT
                NotificationId,
                UserId,
                Title,
                Message,
                NotificationType,
                IsRead,
                CreatedAt
            FROM Notifications
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC
            """,
            new { UserId = userId });
    }

    public async Task MarkAsReadAsync(
        int notificationId,
        int userId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        await connection.ExecuteAsync(
            """
            UPDATE Notifications
            SET IsRead = 1
            WHERE NotificationId = @NotificationId
              AND UserId = @UserId
            """,
            new
            {
                NotificationId = notificationId,
                UserId = userId
            });
    }
}