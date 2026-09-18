using Dapper;
using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;

namespace FinanceManagementSystem.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public CategoryRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Category>> GetByUserIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<Category>(
            """
            SELECT
                CategoryId,
                UserId,
                Name,
                Type
            FROM Categories
            WHERE UserId = @UserId
            ORDER BY Name
            """,
            new { UserId = userId });
    }

    public async Task<int> CreateAsync(Category category)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO Categories
            (
                UserId,
                Name,
                Type
            )
            OUTPUT INSERTED.CategoryId
            VALUES
            (
                @UserId,
                @Name,
                @Type
            )
            """,
            category);
    }

    public async Task<Category?> GetByIdAsync(int categoryId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Category>(
            """
            SELECT
                CategoryId,
                UserId,
                Name,
                Type
            FROM Categories
            WHERE CategoryId = @CategoryId
            """,
            new { CategoryId = categoryId });
    }
}