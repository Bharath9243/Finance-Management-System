using System.Security.Cryptography;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class PasswordResetService : IPasswordResetService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public PasswordResetService(
        IUserRepository userRepository,
        IPasswordResetTokenRepository tokenRepository,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return false;

        var tokenBytes = RandomNumberGenerator.GetBytes(32);

        var token = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        var resetToken = new PasswordResetToken
        {
            UserId = user.UserId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        await _tokenRepository.CreateAsync(resetToken);

        var baseUrl = _configuration["AppSettings:BaseUrl"];

        var resetLink =
            $"{baseUrl}/reset-password?token={Uri.EscapeDataString(token)}";

        await _emailService.SendPasswordResetEmailAsync(
            user.Email,
            resetLink);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(
        string token,
        string newPassword)
    {
        var resetToken =
            await _tokenRepository.GetValidTokenAsync(token);

        if (resetToken == null)
            return false;

        var passwordHash =
            global::BCrypt.Net.BCrypt.HashPassword(newPassword);

        await _userRepository.UpdatePasswordHashAsync(
            resetToken.UserId,
            passwordHash);

        await _tokenRepository.MarkAsUsedAsync(
            resetToken.TokenId);

        return true;
    }
}