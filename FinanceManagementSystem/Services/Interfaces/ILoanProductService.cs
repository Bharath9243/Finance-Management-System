using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Services.Interfaces;

public interface ILoanProductService
{
    Task<IEnumerable<LoanProduct>> GetAllAsync();

    Task<IEnumerable<LoanProduct>> GetActiveAsync();

    Task<LoanProduct?> GetByIdAsync(int loanProductId);

    Task<int> CreateAsync(
        LoanProduct product,
        int adminUserId);

    Task<bool> UpdateAsync(
        LoanProduct product,
        int adminUserId);
}