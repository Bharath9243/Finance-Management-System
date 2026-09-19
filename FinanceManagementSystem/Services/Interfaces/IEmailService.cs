namespace FinanceManagementSystem.Services.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(
        string recipientEmail,
        string resetLink);
}