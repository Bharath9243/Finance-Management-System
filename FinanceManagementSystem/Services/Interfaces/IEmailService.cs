namespace FinanceManagementSystem.Services.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(
        string recipientEmail,
        string resetLink);

    Task SendLoanApplicationEmailAsync(
        string recipientEmail,
        int loanId,
        decimal amount);

    Task SendLoanApprovalEmailAsync(
        string recipientEmail,
        int loanId,
        decimal amount);

    Task SendLoanRejectionEmailAsync(
        string recipientEmail,
        int loanId);

    Task SendLoanPaymentEmailAsync(
        string recipientEmail,
        int loanId,
        decimal amount,
        decimal principalPaid,
        decimal interestPaid,
        decimal outstandingAmount);

    Task SendLoanPaidEmailAsync(
        string recipientEmail,
        int loanId);
}
