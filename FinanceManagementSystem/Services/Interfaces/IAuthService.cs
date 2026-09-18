using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Services.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(
        string name,
        string email,
        string password);

    Task<User?> LoginAsync(
        string email,
        string password);
}