using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> RegisterAsync(
        string name,
        string email,
        string password)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);

        if (existingUser != null)
        {
            return false;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            Role = "User"
        };

        await _userRepository.CreateAsync(user);

        return true;
    }

    public async Task<User?> LoginAsync(
    string email,
    string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        return user;
    }
}
