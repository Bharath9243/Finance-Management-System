using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Services.Interfaces;

public interface ILoanService
{
    Task<int> ApplyForLoanAsync(
    int userId,
    int accountId,
    int loanProductId,
    decimal principalAmount,
    int tenureMonths);

    Task<Loan?> GetMyLoanAsync(
        int userId,
        int loanId);

    Task<IEnumerable<Loan>> GetMyLoansAsync(
        int userId);

    Task<IEnumerable<Loan>> GetAllLoansAsync();

    Task ApproveLoanAsync(
    int adminUserId,
    int loanId);

    Task RejectLoanAsync(
        int adminUserId,
        int loanId);
}

