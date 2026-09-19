using System.Net;
using System.Net.Mail;
using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmailAsync(
        string recipientEmail,
        string resetLink)
    {
        var senderEmail =
            _configuration["EmailSettings:SenderEmail"];

        var appPassword =
            _configuration["EmailSettings:AppPassword"];

        using var message = new MailMessage
        {
            From = new MailAddress(senderEmail!),
            Subject = "Reset Your Password",
            Body = $"""
                   Hello,

                   We received a request to reset your password.

                   Click the link below to reset your password:

                   {resetLink}

                   This link will expire in 30 minutes.

                   If you did not request this, you can ignore this email.

                   Regards,
                   Finance Management System
                   """,
            IsBodyHtml = false
        };

        message.To.Add(recipientEmail);

        using var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(
                senderEmail,
                appPassword)
        };
        await smtp.SendMailAsync(message);
    }
}