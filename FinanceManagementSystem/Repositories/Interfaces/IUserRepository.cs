using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(int userId);

    Task CreateAsync(User user);

    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);

    Task<IEnumerable<User>> GetAllAsync();
}