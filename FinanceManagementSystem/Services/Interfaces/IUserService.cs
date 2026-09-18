using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetByIdAsync(int userId);

    Task<IEnumerable<User>> GetAllAsync();
}