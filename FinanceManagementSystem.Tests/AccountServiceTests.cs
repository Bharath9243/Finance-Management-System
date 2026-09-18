using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services;
using Moq;

namespace FinanceManagementSystem.Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepository = new();
    private readonly Mock<ITransactionRepository> _transactionRepository = new();
    private readonly Mock<ITransferRepository> _transferRepository = new();

    private AccountService CreateService()
    {
        return new AccountService(
            null!,
            _accountRepository.Object,
            _transactionRepository.Object,
            _transferRepository.Object);
    }

    [Fact]
    public async Task CreateAccount_EmptyName_ThrowsException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAccountAsync(1, "", "Savings"));
    }

    [Fact]
    public async Task CreateAccount_InvalidType_ThrowsException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAccountAsync(1, "My Account", "Invalid"));
    }

    [Fact]
    public async Task CreateAccount_DuplicateName_ThrowsException()
    {
        _accountRepository
            .Setup(x => x.GetByUserIdAsync(1))
            .ReturnsAsync(new[]
            {
                new Account
                {
                    AccountId = 1,
                    UserId = 1,
                    AccountName = "Savings",
                    AccountType = "Savings"
                }
            });

        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAccountAsync(1, " savings ", "Savings"));
    }

    [Fact]
    public async Task Deposit_ZeroAmount_ThrowsException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.DepositAsync(1, 1, 0, "Deposit"));
    }

    [Fact]
    public async Task Deposit_OtherUsersAccount_ThrowsException()
    {
        _accountRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Account
            {
                AccountId = 1,
                UserId = 2,
                AccountName = "Savings",
                Balance = 5000
            });

        var service = CreateService();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.DepositAsync(1, 1, 1000, "Deposit"));
    }

    [Fact]
    public async Task Withdraw_InsufficientBalance_ThrowsException()
    {
        _accountRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Account
            {
                AccountId = 1,
                UserId = 1,
                AccountName = "Savings",
                Balance = 500
            });

        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.WithdrawAsync(1, 1, 1000, "Withdrawal"));
    }

    [Fact]
    public async Task Withdraw_ZeroAmount_ThrowsException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.WithdrawAsync(1, 1, 0, "Withdrawal"));
    }

    [Fact]
    public async Task Transfer_SameAccount_ThrowsException()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.TransferAsync(1, 1, 1, 1000, "Transfer"));
    }

    [Fact]
    public async Task Transfer_OtherUsersAccount_ThrowsException()
    {
        _accountRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Account
            {
                AccountId = 1,
                UserId = 1,
                AccountName = "Savings",
                Balance = 5000
            });

        _accountRepository
            .Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(new Account
            {
                AccountId = 2,
                UserId = 2,
                AccountName = "Checking",
                Balance = 2000
            });

        var service = CreateService();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.TransferAsync(1, 1, 2, 1000, "Transfer"));
    }

    [Fact]
    public async Task Transfer_InsufficientBalance_ThrowsException()
    {
        _accountRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Account
            {
                AccountId = 1,
                UserId = 1,
                AccountName = "Savings",
                Balance = 500
            });

        _accountRepository
            .Setup(x => x.GetByIdAsync(2))
            .ReturnsAsync(new Account
            {
                AccountId = 2,
                UserId = 1,
                AccountName = "Checking",
                Balance = 2000
            });

        var service = CreateService();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.TransferAsync(1, 1, 2, 1000, "Transfer"));
    }
}