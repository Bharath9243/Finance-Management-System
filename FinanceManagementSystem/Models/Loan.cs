namespace FinanceManagementSystem.Models;

public class Loan
{
    public int LoanId { get; set; }

    public int UserId { get; set; }

    public int AccountId { get; set; }

    public int LoanProductId { get; set; }

    public decimal PrincipalAmount { get; set; }

    public decimal InterestRate { get; set; }

    public string InterestType { get; set; } = string.Empty;

    public int TenureMonths { get; set; }

    public decimal TotalInterest { get; set; }

    public decimal TotalPayable { get; set; }

    public decimal OutstandingAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? StartDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }
}