namespace FinanceManagementSystem.Services.Interfaces;

public interface ILoanCalculationService
{
    (decimal TotalInterest, decimal TotalPayable)
        CalculateLoan(
            decimal principalAmount,
            decimal interestRate,
            string interestType,
            int tenureMonths);
}