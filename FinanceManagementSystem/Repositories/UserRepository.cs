using Dapper;
using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;

namespace FinanceManagementSystem.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public UserRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT UserId, Name, Email, PasswordHash, Role, CreatedAt
            FROM Users
            WHERE Email = @Email
            """;

        return await connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { Email = email });
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
        SELECT UserId, Name, Email, PasswordHash, Role, CreatedAt
        FROM Users
        WHERE UserId = @UserId
        """;

        return await connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { UserId = userId });
    }

    public async Task CreateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Users
                (Name, Email, PasswordHash, Role)
            VALUES
                (@Name, @Email, @PasswordHash, @Role)
            """;

        await connection.ExecuteAsync(sql, user);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<User>(
            """
        SELECT UserId, Name, Email, PasswordHash, Role, CreatedAt
        FROM Users
        WHERE Role = @Role
        """,
            new { Role = role });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
        SELECT UserId, Name, Email, PasswordHash, Role, CreatedAt
        FROM Users
        ORDER BY CreatedAt DESC
        """;

        return await connection.QueryAsync<User>(sql);
    }

    public async Task UpdatePasswordHashAsync(
    int userId,
    string passwordHash)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
        UPDATE Users
        SET PasswordHash = @PasswordHash
        WHERE UserId = @UserId
        """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                UserId = userId,
                PasswordHash = passwordHash
            });
    }
}

