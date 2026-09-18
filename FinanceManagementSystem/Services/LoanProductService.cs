using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class LoanProductService : ILoanProductService
{
    private readonly DbConnectionFactory _connectionFactory;
    private readonly ILoanProductRepository _repository;
    private readonly IAdminActivityService _adminActivityService;

    public LoanProductService(
        DbConnectionFactory connectionFactory,
        ILoanProductRepository repository,
        IAdminActivityService adminActivityService)
    {
        _connectionFactory = connectionFactory;
        _repository = repository;
        _adminActivityService = adminActivityService;
    }

    public Task<IEnumerable<LoanProduct>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public Task<IEnumerable<LoanProduct>> GetActiveAsync()
    {
        return _repository.GetActiveAsync();
    }

    public Task<LoanProduct?> GetByIdAsync(int loanProductId)
    {
        return _repository.GetByIdAsync(loanProductId);
    }

    public async Task<int> CreateAsync(
        LoanProduct product,
        int adminUserId)
    {
        Validate(product);

        product.CreatedAt = DateTime.Now;
        product.IsActive = true;

        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var dbTransaction = connection.BeginTransaction();

        try
        {
            var loanProductId =
                await _repository.CreateAsync(
                    product,
                    dbTransaction);

            await _adminActivityService.CreateAsync(
                new AdminActivity
                {
                    AdminUserId = adminUserId,
                    Action = "Create",
                    EntityType = "LoanProduct",
                    EntityId = loanProductId,
                    Description =
                        $"Created loan product '{product.Name}'.",
                    CreatedAt = DateTime.Now
                },
                dbTransaction);

            dbTransaction.Commit();

            return loanProductId;
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
    }

    public async Task<bool> UpdateAsync(
        LoanProduct product,
        int adminUserId)
    {
        Validate(product);

        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var dbTransaction = connection.BeginTransaction();

        try
        {
            var updated =
                await _repository.UpdateAsync(
                    product,
                    dbTransaction);

            if (!updated)
            {
                dbTransaction.Rollback();
                return false;
            }

            await _adminActivityService.CreateAsync(
                new AdminActivity
                {
                    AdminUserId = adminUserId,
                    Action = "Update",
                    EntityType = "LoanProduct",
                    EntityId = product.LoanProductId,
                    Description =
                        $"Updated loan product '{product.Name}'.",
                    CreatedAt = DateTime.Now
                },
                dbTransaction);

            dbTransaction.Commit();

            return true;
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
    }

    private static void Validate(LoanProduct product)
    {
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException(
                "Loan product name is required.");

        if (product.InterestRate < 0)
            throw new ArgumentException(
                "Interest rate cannot be negative.");

        if (product.MaxAmount <= 0)
            throw new ArgumentException(
                "Maximum amount must be greater than zero.");

        if (product.MaxTenureMonths <= 0)
            throw new ArgumentException(
                "Maximum tenure must be greater than zero.");

        if (product.InterestType is not ("Simple" or "Compound"))
            throw new ArgumentException(
                "Invalid interest type.");
    }
}