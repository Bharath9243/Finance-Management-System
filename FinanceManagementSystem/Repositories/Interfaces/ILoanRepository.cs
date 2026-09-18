using FinanceManagementSystem.Models;
using System.Data;

namespace FinanceManagementSystem.Repositories.Interfaces;

public interface ILoanRepository
{
    Task<int> CreateAsync(
        Loan loan,
        IDbTransaction dbTransaction);

    Task<Loan?> GetByIdAsync(int loanId);

    Task<IEnumerable<Loan>> GetByUserIdAsync(int userId);

    Task<IEnumerable<Loan>> GetAllAsync();

    Task UpdateStatusAsync(
        int loanId,
        string status,
        DateTime? approvedAt,
        DateTime? startDate,
        IDbTransaction dbTransaction);

    Task UpdateOutstandingAmountAsync(
        int loanId,
        decimal outstandingAmount,
        string status,
        IDbTransaction dbTransaction);
}