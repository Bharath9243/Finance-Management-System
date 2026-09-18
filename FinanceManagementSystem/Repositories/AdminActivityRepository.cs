using Dapper;
using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using System.Data;

namespace FinanceManagementSystem.Repositories;

public class AdminActivityRepository : IAdminActivityRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public AdminActivityRepository(
        DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task CreateAsync(
        AdminActivity activity,
        IDbTransaction dbTransaction)
    {
        await dbTransaction.Connection!.ExecuteAsync(
            """
            INSERT INTO AdminActivities
            (
                AdminUserId,
                Action,
                EntityType,
                EntityId,
                Description,
                CreatedAt
            )
            VALUES
            (
                @AdminUserId,
                @Action,
                @EntityType,
                @EntityId,
                @Description,
                @CreatedAt
            )
            """,
            activity,
            dbTransaction);
    }

    public async Task<IEnumerable<AdminActivity>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<AdminActivity>(
            """
            SELECT
                AdminActivityId,
                AdminUserId,
                Action,
                EntityType,
                EntityId,
                Description,
                CreatedAt
            FROM AdminActivities
            ORDER BY CreatedAt DESC
            """);
    }
}

