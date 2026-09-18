namespace FinanceManagementSystem.Models;

public class Transaction
{
    public int TransactionId { get; set; }

    public int AccountId { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public int? TransferId { get; set; }

    public int? LoanId { get; set; }

    public int? LoanPaymentId { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime TransactionDate { get; set; }
}