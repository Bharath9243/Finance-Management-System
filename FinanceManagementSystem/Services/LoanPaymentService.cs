using FinanceManagementSystem.Data;
using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class LoanPaymentService : ILoanPaymentService
{
    private readonly DbConnectionFactory _connectionFactory;
    private readonly ILoanRepository _loanRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ILoanPaymentRepository _loanPaymentRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly INotificationRepository _notificationRepository;

    public LoanPaymentService(
        DbConnectionFactory connectionFactory,
        ILoanRepository loanRepository,
        IAccountRepository accountRepository,
        ILoanPaymentRepository loanPaymentRepository,
        ITransactionRepository transactionRepository,
        INotificationRepository notificationRepository)
    {
        _connectionFactory = connectionFactory;
        _loanRepository = loanRepository;
        _accountRepository = accountRepository;
        _loanPaymentRepository = loanPaymentRepository;
        _transactionRepository = transactionRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task MakePaymentAsync(
        int userId,
        int loanId,
        int accountId,
        decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException(
                "Payment amount must be greater than zero.");

        var loan =
            await _loanRepository.GetByIdAsync(loanId);

        if (loan == null)
            throw new InvalidOperationException(
                "Loan not found.");

        if (loan.UserId != userId)
            throw new UnauthorizedAccessException(
                "You can only pay your own loans.");

        if (loan.Status != "Active")
            throw new InvalidOperationException(
                "Only active loans can receive payments.");

        var account =
            await _accountRepository.GetByIdAsync(accountId);

        if (account == null)
            throw new InvalidOperationException(
                "Account not found.");

        if (account.UserId != userId)
            throw new UnauthorizedAccessException(
                "You can only use your own account.");

        if (amount > loan.OutstandingAmount)
            throw new InvalidOperationException(
                "Payment cannot exceed the outstanding amount.");

        if (account.Balance < amount)
            throw new InvalidOperationException(
                "Insufficient account balance.");

        /*
         * Calculate the remaining interest.
         *
         * TotalInterest represents the total interest
         * calculated when the loan was created.
         *
         * Previous payments reduce the interest portion first.
         */
        var previousPayments =
            await _loanPaymentRepository
                .GetByLoanIdAsync(loanId);

        var interestAlreadyPaid =
            previousPayments.Sum(p => p.InterestPaid);

        var remainingInterest =
            Math.Max(
                0,
                loan.TotalInterest - interestAlreadyPaid);

        var interestPaid =
            Math.Min(amount, remainingInterest);

        var principalPaid =
            amount - interestPaid;

        var newOutstanding =
            loan.OutstandingAmount - amount;

        newOutstanding =
            Math.Round(
                newOutstanding,
                2,
                MidpointRounding.AwayFromZero);

        var newStatus =
            newOutstanding == 0
                ? "Paid"
                : "Active";

        using var connection =
            _connectionFactory.CreateConnection();

        connection.Open();

        using var dbTransaction =
            connection.BeginTransaction();

        try
        {
            // 1. Debit user's account
            await _accountRepository.UpdateBalanceAsync(
                accountId,
                account.Balance - amount,
                dbTransaction);

            // 2. Update loan outstanding amount
            await _loanRepository.UpdateOutstandingAmountAsync(
                loanId,
                newOutstanding,
                newStatus,
                dbTransaction);

            // 3. Create loan payment record
            var paymentId =
                await _loanPaymentRepository.CreateAsync(
                    new LoanPayment
                    {
                        LoanId = loanId,
                        AccountId = accountId,
                        Amount = amount,
                        PrincipalPaid = principalPaid,
                        InterestPaid = interestPaid
                    },
                    dbTransaction);

            // 4. Create transaction history record
            await _transactionRepository.CreateAsync(
                new Transaction
                {
                    AccountId = accountId,
                    LoanId = loanId,
                    LoanPaymentId = paymentId,
                    TransactionType = "LoanPayment",
                    Amount = amount,
                    Description = "Loan payment"
                },
                dbTransaction);

            // 5. Create payment notification
            await _notificationRepository.CreateAsync(
                new Notification
                {
                    UserId = loan.UserId,
                    Title = "Loan Payment Successful",
                    Message =
                        $"Your payment of ₹{amount:N2} for loan #{loan.LoanId} " +
                        $"was successful. Outstanding amount: ₹{newOutstanding:N2}.",
                    NotificationType = "LoanPayment"
                },
                dbTransaction);

            // 6. If the loan is fully paid, create another notification
            if (newOutstanding == 0)
            {
                await _notificationRepository.CreateAsync(
                    new Notification
                    {
                        UserId = loan.UserId,
                        Title = "Loan Fully Paid",
                        Message =
                            $"Congratulations! Your loan #{loan.LoanId} " +
                            $"has been fully paid.",
                        NotificationType = "LoanPaid"
                    },
                    dbTransaction);
            }

            dbTransaction.Commit();
        }
        catch
        {
            dbTransaction.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<LoanPayment>>
        GetPaymentHistoryAsync(
            int userId,
            int loanId)
    {
        var loan =
            await _loanRepository.GetByIdAsync(loanId);

        if (loan == null)
            throw new InvalidOperationException(
                "Loan not found.");

        if (loan.UserId != userId)
            throw new UnauthorizedAccessException(
                "You can only access your own loan payments.");

        return await _loanPaymentRepository
            .GetByLoanIdAsync(loanId);
    }
}
