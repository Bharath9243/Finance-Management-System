using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services;
using Moq;

namespace FinanceManagementSystem.Tests;

public class LoanPaymentServiceTests
{
    private readonly Mock<ILoanRepository> _loanRepository = new();
    private readonly Mock<IAccountRepository> _accountRepository = new();
    private readonly Mock<ILoanPaymentRepository> _loanPaymentRepository = new();
    private readonly Mock<ITransactionRepository> _transactionRepository = new();
    private readonly Mock<INotificationRepository> _notificationRepository = new();

    private LoanPaymentService CreateService()
    {
        return new LoanPaymentService(
            null!,
            _loanRepository.Object,
            _accountRepository.Object,
            _loanPaymentRepository.Object,
            _transactionRepository.Object,
            _notificationRepository.Object);
    }

    [Fact]
    public async Task MakePayment_ZeroAmount_ThrowsException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.MakePaymentAsync(1, 1, 1, 0));
    }

    [Fact]
    public async Task MakePayment_LoanNotFound_ThrowsException()
    {
        _loanRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Loan?)null);

        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.MakePaymentAsync(1, 1, 1, 1000));
    }

    [Fact]
    public async Task MakePayment_OtherUsersLoan_ThrowsException()
    {
        _loanRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Loan
            {
                LoanId = 1,
                UserId = 2,
                Status = "Active",
                OutstandingAmount = 5000,
                TotalInterest = 500
            });

        var service = CreateService();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.MakePaymentAsync(1, 1, 1, 1000));
    }

    [Fact]
    public async Task MakePayment_InactiveLoan_ThrowsException()
    {
        _loanRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Loan
            {
                LoanId = 1,
                UserId = 1,
                Status = "Paid",
                OutstandingAmount = 0,
                TotalInterest = 500
            });

        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.MakePaymentAsync(1, 1, 1, 1000));
    }

    [Fact]
    public async Task MakePayment_ExceedsOutstandingAmount_ThrowsException()
    {
        _loanRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Loan
            {
                LoanId = 1,
                UserId = 1,
                Status = "Active",
                OutstandingAmount = 5000,
                TotalInterest = 500
            });

        _accountRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Account
            {
                AccountId = 1,
                UserId = 1,
                Balance = 10000
            });

        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.MakePaymentAsync(1, 1, 1, 6000));
    }

    [Fact]
    public async Task MakePayment_InsufficientBalance_ThrowsException()
    {
        _loanRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Loan
            {
                LoanId = 1,
                UserId = 1,
                Status = "Active",
                OutstandingAmount = 5000,
                TotalInterest = 500
            });

        _accountRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Account
            {
                AccountId = 1,
                UserId = 1,
                Balance = 500
            });

        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.MakePaymentAsync(1, 1, 1, 1000));
    }
}

