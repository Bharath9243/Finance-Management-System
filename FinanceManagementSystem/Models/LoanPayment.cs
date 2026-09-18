namespace FinanceManagementSystem.Models;

public class LoanPayment
{
    public int PaymentId { get; set; }

    public int LoanId { get; set; }

    public int AccountId { get; set; }

    public decimal Amount { get; set; }

    public decimal PrincipalPaid { get; set; }

    public decimal InterestPaid { get; set; }

    public DateTime PaymentDate { get; set; }
}