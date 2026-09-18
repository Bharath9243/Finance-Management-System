using FinanceManagementSystem.Models;
using System.Data;

namespace FinanceManagementSystem.Repositories.Interfaces;

public interface ILoanPaymentRepository
{
    Task<int> CreateAsync(
        LoanPayment payment,
        IDbTransaction dbTransaction);

    Task<IEnumerable<LoanPayment>> GetByLoanIdAsync(
        int loanId);
}
