using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class LoanCalculationService : ILoanCalculationService
{
    public (decimal TotalInterest, decimal TotalPayable)
        CalculateLoan(
            decimal principalAmount,
            decimal interestRate,
            string interestType,
            int tenureMonths)
    {
        if (principalAmount <= 0)
            throw new ArgumentException(
                "Principal amount must be greater than zero.");

        if (interestRate < 0)
            throw new ArgumentException(
                "Interest rate cannot be negative.");

        if (tenureMonths <= 0)
            throw new ArgumentException(
                "Tenure must be greater than zero.");

        if (interestType != "Simple" &&
            interestType != "Compound")
        {
            throw new ArgumentException(
                "Interest type must be Simple or Compound.");
        }

        decimal totalPayable;

        if (interestType == "Simple")
        {
            // Time is converted from months to years.
            decimal timeInYears = tenureMonths / 12m;

            decimal interest =
                principalAmount *
                (interestRate / 100m) *
                timeInYears;

            totalPayable = principalAmount + interest;
        }
        else
        {
            // Compound interest is compounded monthly.
            const int compoundsPerYear = 12;

            decimal monthlyRate =
                interestRate / (100m * compoundsPerYear);

            totalPayable =
                principalAmount *
                (decimal)Math.Pow(
                    (double)(1 + monthlyRate),
                    tenureMonths);
        }

        totalPayable = Math.Round(
            totalPayable,
            2,
            MidpointRounding.AwayFromZero);

        decimal totalInterest =
            Math.Round(
                totalPayable - principalAmount,
                2,
                MidpointRounding.AwayFromZero);

        return (totalInterest, totalPayable);
    }
}