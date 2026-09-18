using FinanceManagementSystem.Models;
using System.Data;

namespace FinanceManagementSystem.Repositories.Interfaces;

public interface ILoanProductRepository
{
    Task<IEnumerable<LoanProduct>> GetAllAsync();

    Task<IEnumerable<LoanProduct>> GetActiveAsync();

    Task<LoanProduct?> GetByIdAsync(int loanProductId);

    Task<int> CreateAsync(
        LoanProduct product,
        IDbTransaction dbTransaction);

    Task<bool> UpdateAsync(
        LoanProduct product,
        IDbTransaction dbTransaction);
}