using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Repositories.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task CreateAsync(PasswordResetToken resetToken);

    Task<PasswordResetToken?> GetValidTokenAsync(string token);

    Task MarkAsUsedAsync(int tokenId);
}