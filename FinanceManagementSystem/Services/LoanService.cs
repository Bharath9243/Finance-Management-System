using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class LoanService : ILoanService
{
    private readonly DbConnectionFactory _connectionFactory;
    private readonly IUserRepository _userRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILoanCalculationService _loanCalculationService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ILoanProductService _loanProductService;
    private readonly IAdminActivityService _adminActivityService;

    public LoanService(
    DbConnectionFactory connectionFactory,
    IUserRepository userRepository,
    ILoanRepository loanRepository,
    IAccountRepository accountRepository,
    ILoanCalculationService loanCalculationService,
    ITransactionRepository transactionRepository,
    INotificationRepository notificationRepository,
    ILoanProductService loanProductService,
    IAdminActivityService adminActivityService)
    {
        _connectionFactory = connectionFactory;
        _userRepository = userRepository;
        _loanRepository = loanRepository;
        _accountRepository = accountRepository;
        _loanCalculationService = loanCalculationService;
        _transactionRepository = transactionRepository;
        _notificationRepository = notificationRepository;
        _loanProductService = loanProductService;
        _adminActivityService = adminActivityService;
    }

    public async Task<int> ApplyForLoanAsync(
    int userId,
    int accountId,
    int loanProductId,
    decimal principalAmount,
    int tenureMonths)
    {
        if (principalAmount <= 0)
            throw new ArgumentException(
                "Loan amount must be greater than zero.");

        if (tenureMonths <= 0)
            throw new ArgumentException(
                "Tenure must be greater than zero.");

        // Get the selected loan product.
        var loanProduct =
            await _loanProductService.GetByIdAsync(loanProductId);

        if (loanProduct == null)
            throw new InvalidOperationException(
                "Loan product not found.");

        if (!loanProduct.IsActive)
            throw new InvalidOperationException(
                "The selected loan product is not active.");

        // Validate amount against the product limit.
        if (principalAmount > loanProduct.MaxAmount)
            throw new ArgumentException(
                $"Loan amount cannot exceed ₹{loanProduct.MaxAmount:N2}.");

        // Validate tenure against the product limit.
        if (tenureMonths > loanProduct.MaxTenureMonths)
            throw new ArgumentException(
                $"Tenure cannot exceed {loanProduct.MaxTenureMonths} months.");

        // Make sure the account exists and belongs to the user.
        var account =
            await _accountRepository.GetByIdAsync(accountId);

        if (account == null)
            throw new InvalidOperationException(
                "Account not found.");

        if (account.UserId != userId)
            throw new UnauthorizedAccessException(
                "You can only apply for a loan using your own account.");

        // Use the terms defined by the loan product.
        var interestRate = loanProduct.InterestRate;
        var interestType = loanProduct.InterestType;

        // Calculate interest and total payable.
        var calculation =
            _loanCalculationService.CalculateLoan(
                principalAmount,
                interestRate,
                interestType,
                tenureMonths);

        var loan = new Loan
        {
            UserId = userId,
            AccountId = accountId,
            LoanProductId = loanProductId,

            PrincipalAmount = principalAmount,

            // Snapshot the product terms.
            InterestRate = interestRate,
            InterestType = interestType,
            TenureMonths = tenureMonths,

            TotalInterest = calculation.TotalInterest,
            TotalPayable = calculation.TotalPayable,
            OutstandingAmount = calculation.TotalPayable,

            Status = "Pending"
        };

        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var dbTransaction = connection.BeginTransaction();

        try
        {
            var loanId =
                await _loanRepository.CreateAsync(
                    loan,
                    dbTransaction);

            await _notificationRepository.CreateAsync(
                new Notification
                {
                    UserId = userId,
                    Title = "Loan Application Submitted",
                    Message =
                        $"Your loan application #{loanId} has been submitted " +
                        $"and is awaiting approval.",
                    NotificationType = "LoanApplication"
                },
                dbTransaction);

            var adminUsers =
                await _userRepository.GetUsersByRoleAsync("Admin");

            foreach (var admin in adminUsers)
            {
                await _notificationRepository.CreateAsync(
                    new Notification
                    {
                        UserId = admin.UserId,
                        Title = "New Loan Application",
                        Message =
                            $"User #{userId} has submitted loan application " +
                            $"#{loanId} for ₹{principalAmount:N2} " +
                            $"and it is awaiting your review.",
                        NotificationType = "LoanApplication"
                    },
                    dbTransaction);
            }

            dbTransaction.Commit();

            return loanId;
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
    }

    public async Task<Loan?> GetMyLoanAsync(
        int userId,
        int loanId)
    {
        var loan =
            await _loanRepository.GetByIdAsync(loanId);

        if (loan == null)
            return null;

        if (loan.UserId != userId)
            throw new UnauthorizedAccessException(
                "You can only access your own loans.");

        return loan;
    }

    public async Task<IEnumerable<Loan>> GetMyLoansAsync(
        int userId)
    {
        return await _loanRepository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<Loan>> GetAllLoansAsync()
    {
        return await _loanRepository.GetAllAsync();
    }

    public async Task ApproveLoanAsync(
    int adminUserId,
    int loanId)
    {
        var loan =
            await _loanRepository.GetByIdAsync(loanId);

        if (loan == null)
            throw new InvalidOperationException(
                "Loan not found.");

        if (loan.Status != "Pending")
            throw new InvalidOperationException(
                "Only pending loans can be approved.");

        // Verify that the loan's account belongs to the loan owner.
        var account =
            await _accountRepository.GetByIdAsync(loan.AccountId);

        if (account == null)
            throw new InvalidOperationException(
                "Loan account not found.");

        if (account.UserId != loan.UserId)
            throw new InvalidOperationException(
                "The loan account does not belong to the loan owner.");

        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var dbTransaction = connection.BeginTransaction();

        try
        {
            // 1. Credit the user's account with the loan principal.
            await _accountRepository.UpdateBalanceAsync(
                loan.AccountId,
                account.Balance + loan.PrincipalAmount,
                dbTransaction);

            // 2. Change loan status to Active.
            var now = DateTime.UtcNow;

            await _loanRepository.UpdateStatusAsync(
                loanId,
                "Active",
                now,
                now,
                dbTransaction);

            await _transactionRepository.CreateAsync(
    new Transaction
    {
        AccountId = loan.AccountId,
        LoanId = loan.LoanId,
        TransactionType = "LoanDisbursement",
        Amount = loan.PrincipalAmount,
        Description = "Loan disbursement"
    },
    dbTransaction);

            // 3. Record the loan disbursement.
            await _notificationRepository.CreateAsync(
                new Notification
                {
                    UserId = loan.UserId,
                    Title = "Loan Approved",
                    Message = $"Your loan #{loan.LoanId} has been approved and ₹{loan.PrincipalAmount:N2} has been credited to your account.",
                    NotificationType = "LoanApproval"
                },
                dbTransaction);

            await _adminActivityService.CreateAsync(
    new AdminActivity
    {
        AdminUserId = adminUserId,
        Action = "Approve",
        EntityType = "Loan",
        EntityId = loan.LoanId,
        Description =
            $"Loan #{loan.LoanId} approved and ₹{loan.PrincipalAmount:N2} disbursed.",
        CreatedAt = DateTime.UtcNow
    },
    dbTransaction);

            dbTransaction.Commit();
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
    }

    public async Task RejectLoanAsync(int adminUserId, int loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);

        if (loan == null)
            throw new InvalidOperationException("Loan not found.");

        if (loan.Status != "Pending")
            throw new InvalidOperationException("Only pending loans can be rejected.");

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        using var dbTransaction = connection.BeginTransaction();

        try
        {
            await _loanRepository.UpdateStatusAsync(
                loan.LoanId,
                "Rejected",
                null,
                null,
                dbTransaction);

            await _notificationRepository.CreateAsync(
                new Notification
                {
                    UserId = loan.UserId,
                    Title = "Loan Rejected",
                    Message = $"Your loan application #{loan.LoanId} has been rejected.",
                    NotificationType = "LoanRejection"
                },
                dbTransaction);

            await _adminActivityService.CreateAsync(
    new AdminActivity
    {
        AdminUserId = adminUserId,
        Action = "Reject",
        EntityType = "Loan",
        EntityId = loan.LoanId,
        Description =
            $"Loan #{loan.LoanId} rejected.",
        CreatedAt = DateTime.UtcNow
    },
    dbTransaction);

            dbTransaction.Commit();
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
    }
}
