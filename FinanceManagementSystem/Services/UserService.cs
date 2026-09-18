using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<User?> GetByIdAsync(int userId)
    {
        return _userRepository.GetByIdAsync(userId);
    }

    public Task<IEnumerable<User>> GetAllAsync()
    {
        return _userRepository.GetAllAsync();
    }
}