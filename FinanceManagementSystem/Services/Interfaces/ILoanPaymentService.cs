namespace FinanceManagementSystem.Services.Interfaces;

using FinanceManagementSystem.Models;

public interface ILoanPaymentService
{
    Task MakePaymentAsync(
        int userId,
        int loanId,
        int accountId,
        decimal amount);

    Task<IEnumerable<LoanPayment>> GetPaymentHistoryAsync(
        int userId,
        int loanId);
}