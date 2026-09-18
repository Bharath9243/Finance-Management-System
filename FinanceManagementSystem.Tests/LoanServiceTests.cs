using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services;
using FinanceManagementSystem.Services.Interfaces;
using Moq;

namespace FinanceManagementSystem.Tests;

public class LoanServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<ILoanRepository> _loanRepository = new();
    private readonly Mock<IAccountRepository> _accountRepository = new();
    private readonly Mock<ILoanCalculationService> _loanCalculationService = new();
    private readonly Mock<ITransactionRepository> _transactionRepository = new();
    private readonly Mock<INotificationRepository> _notificationRepository = new();
    private readonly Mock<ILoanProductService> _loanProductService = new();
    private readonly Mock<IAdminActivityService> _adminActivityService = new();

    private LoanService CreateService()
    {
        return new LoanService(
            null!,
            _userRepository.Object,
            _loanRepository.Object,
            _accountRepository.Object,
            _loanCalculationService.Object,
            _transactionRepository.Object,
            _notificationRepository.Object,
            _loanProductService.Object,
            _adminActivityService.Object);
    }

    [Fact]
    public async Task ApplyForLoan_ZeroAmount_ThrowsException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ApplyForLoanAsync(1, 1, 1, 0, 12));
    }

    [Fact]
    public async Task ApplyForLoan_ZeroTenure_ThrowsException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ApplyForLoanAsync(1, 1, 1, 10000, 0));
    }

    [Fact]
    public async Task ApplyForLoan_InactiveProduct_ThrowsException()
    {
        _loanProductService
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new LoanProduct
            {
                LoanProductId = 1,
                Name = "Old Loan",
                IsActive = false,
                MaxAmount = 100000,
                MaxTenureMonths = 12
            });

        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ApplyForLoanAsync(1, 1, 1, 10000, 12));
    }

    [Fact]
    public async Task ApplyForLoan_AmountExceedsMaximum_ThrowsException()
    {
        _loanProductService
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new LoanProduct
            {
                LoanProductId = 1,
                Name = "Personal Loan",
                IsActive = true,
                MaxAmount = 50000,
                MaxTenureMonths = 12
            });

        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ApplyForLoanAsync(1, 1, 1, 60000, 12));
    }

    [Fact]
    public async Task ApplyForLoan_TenureExceedsMaximum_ThrowsException()
    {
        _loanProductService
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new LoanProduct
            {
                LoanProductId = 1,
                Name = "Personal Loan",
                IsActive = true,
                MaxAmount = 50000,
                MaxTenureMonths = 12
            });

        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.ApplyForLoanAsync(1, 1, 1, 10000, 24));
    }

    [Fact]
    public async Task ApplyForLoan_AccountBelongsToAnotherUser_ThrowsException()
    {
        _loanProductService
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new LoanProduct
            {
                LoanProductId = 1,
                Name = "Personal Loan",
                IsActive = true,
                MaxAmount = 50000,
                MaxTenureMonths = 12,
                InterestRate = 10,
                InterestType = "Simple"
            });

        _accountRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Account
            {
                AccountId = 1,
                UserId = 2,
                Balance = 10000
            });

        var service = CreateService();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.ApplyForLoanAsync(1, 1, 1, 10000, 12));
    }

    [Fact]
    public async Task GetMyLoan_OtherUsersLoan_ThrowsException()
    {
        _loanRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Loan
            {
                LoanId = 1,
                UserId = 2,
                Status = "Active"
            });

        var service = CreateService();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.GetMyLoanAsync(1, 1));
    }

    [Fact]
    public async Task ApproveLoan_NonPendingLoan_ThrowsException()
    {
        _loanRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Loan
            {
                LoanId = 1,
                UserId = 1,
                AccountId = 1,
                Status = "Active"
            });

        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ApproveLoanAsync(99, 1));
    }

    [Fact]
    public async Task RejectLoan_NonPendingLoan_ThrowsException()
    {
        _loanRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Loan
            {
                LoanId = 1,
                UserId = 1,
                Status = "Rejected"
            });

        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RejectLoanAsync(99, 1));
    }
}