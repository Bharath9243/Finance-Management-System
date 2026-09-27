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
        var body = BuildEmail(
            "Reset Your Password",
            $"""
            <p>We received a request to reset your password.</p>

            <p>
                Click the button below to create a new password.
            </p>

            <div style="text-align:center; margin:30px 0;">
                <a href="{resetLink}"
                   style="
                       background-color:#2563eb;
                       color:#ffffff;
                       padding:12px 24px;
                       text-decoration:none;
                       border-radius:6px;
                       font-weight:bold;
                       display:inline-block;">
                    Reset Password
                </a>
            </div>

            <p style="font-size:13px; color:#6b7280;">
                This link will expire in 30 minutes.
            </p>

            <p style="font-size:13px; color:#6b7280;">
                If you did not request this password reset,
                you can safely ignore this email.
            </p>
            """);

        await SendEmailAsync(
            recipientEmail,
            "Reset Your Password",
            body);
    }

    public async Task SendLoanApplicationEmailAsync(
        string recipientEmail,
        int loanId,
        decimal amount)
    {
        var body = BuildEmail(
            "Loan Application Submitted",
            $"""
            <p>
                Your loan application has been submitted successfully
                and is awaiting admin approval.
            </p>

            {BuildDetailsTable(
                ("Loan ID", $"#{loanId}"),
                ("Requested Amount", $"₹{amount:N2}"),
                ("Status", "Pending"))}

            <p>
                You will receive another notification once your application
                has been reviewed.
            </p>
            """);

        await SendEmailAsync(
            recipientEmail,
            "Loan Application Submitted",
            body);
    }

    public async Task SendLoanApprovalEmailAsync(
        string recipientEmail,
        int loanId,
        decimal amount)
    {
        var body = BuildEmail(
            "Loan Approved",
            $"""
            <p>
                Your loan application has been approved successfully.
            </p>

            {BuildDetailsTable(
                ("Loan ID", $"#{loanId}"),
                ("Disbursed Amount", $"₹{amount:N2}"),
                ("Status", "Active"))}

            <p>
                The loan amount has been credited to your account.
            </p>
            """);

        await SendEmailAsync(
            recipientEmail,
            "Loan Approved",
            body);
    }

    public async Task SendLoanRejectionEmailAsync(
        string recipientEmail,
        int loanId)
    {
        var body = BuildEmail(
            "Loan Application Rejected",
            $"""
            <p>
                Your loan application has been reviewed and rejected.
            </p>

            {BuildDetailsTable(
                ("Loan ID", $"#{loanId}"),
                ("Status", "Rejected"))}

            <p>
                Please contact the administrator if you need
                more information.
            </p>
            """);

        await SendEmailAsync(
            recipientEmail,
            "Loan Application Rejected",
            body);
    }

    public async Task SendLoanPaymentEmailAsync(
        string recipientEmail,
        int loanId,
        decimal amount,
        decimal principalPaid,
        decimal interestPaid,
        decimal outstandingAmount)
    {
        var body = BuildEmail(
            "Loan Payment Successful",
            $"""
            <p>
                Your loan payment was completed successfully.
            </p>

            {BuildDetailsTable(
                ("Loan ID", $"#{loanId}"),
                ("Payment Amount", $"₹{amount:N2}"),
                ("Principal Paid", $"₹{principalPaid:N2}"),
                ("Interest Paid", $"₹{interestPaid:N2}"),
                ("Outstanding Amount", $"₹{outstandingAmount:N2}"))}
            """);

        await SendEmailAsync(
            recipientEmail,
            "Loan Payment Successful",
            body);
    }

    public async Task SendLoanPaidEmailAsync(
        string recipientEmail,
        int loanId)
    {
        var body = BuildEmail(
            "Loan Fully Paid",
            $"""
            <p>
                Congratulations! Your loan has been fully paid.
            </p>

            {BuildDetailsTable(
                ("Loan ID", $"#{loanId}"),
                ("Status", "Paid"),
                ("Outstanding Amount", "₹0.00"))}

            <p>
                There is no outstanding amount remaining on this loan.
            </p>
            """);

        await SendEmailAsync(
            recipientEmail,
            "Loan Fully Paid",
            body);
    }

    private async Task SendEmailAsync(
        string recipientEmail,
        string subject,
        string body)
    {
        var senderEmail =
            _configuration["EmailSettings:SenderEmail"];

        var appPassword =
            _configuration["EmailSettings:AppPassword"];

        using var message = new MailMessage
        {
            From = new MailAddress(senderEmail!),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(recipientEmail);

        using var smtp = new SmtpClient(
            "smtp.gmail.com",
            587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(
                senderEmail,
                appPassword)
        };

        await smtp.SendMailAsync(message);
    }

    private static string BuildEmail(
        string title,
        string content)
    {
        return $"""
        <!DOCTYPE html>
        <html>
        <body style="
            margin:0;
            padding:0;
            background-color:#f4f6f8;
            font-family:Arial,Helvetica,sans-serif;
            color:#333333;">

            <div style="
                max-width:600px;
                margin:30px auto;
                background-color:#ffffff;
                border-radius:10px;
                overflow:hidden;
                border:1px solid #e5e7eb;">

                <!-- Header -->
                <div style="
                    background-color:#2563eb;
                    color:#ffffff;
                    padding:24px;
                    text-align:center;">

                    <h1 style="
                        margin:0;
                        font-size:22px;">
                        Finance Management System
                    </h1>
                </div>

                <!-- Content -->
                <div style="
                    padding:30px;">

                    <h2 style="
                        margin:0 0 20px 0;
                        color:#1f2937;
                        font-size:20px;">
                        {title}
                    </h2>

                    <div style="
                        font-size:15px;
                        line-height:1.6;
                        color:#374151;">

                        {content}

                    </div>

                </div>

                <!-- Footer -->
                <div style="
                    background-color:#f8fafc;
                    padding:18px;
                    text-align:center;
                    color:#6b7280;
                    font-size:12px;">

                    <p style="margin:0;">
                        This is an automated email from
                        Finance Management System.
                    </p>

                    <p style="margin:6px 0 0;">
                        Please do not reply to this email.
                    </p>

                </div>

            </div>

        </body>
        </html>
        """;
    }

    private static string BuildDetailsTable(
        params (string Label, string Value)[] details)
    {
        var rows = string.Join(
            "",
            details.Select(detail => $"""
                <tr>
                    <td style="
                        padding:11px;
                        border-bottom:1px solid #e5e7eb;
                        color:#6b7280;
                        font-weight:bold;">
                        {detail.Label}
                    </td>

                    <td style="
                        padding:11px;
                        border-bottom:1px solid #e5e7eb;
                        text-align:right;
                        color:#111827;">
                        {detail.Value}
                    </td>
                </tr>
                """));

        return $"""
        <table style="
            width:100%;
            border-collapse:collapse;
            margin:22px 0;
            font-size:14px;">

            {rows}

        </table>
        """;
    }
}
