namespace FinanceManagementSystem.Models;

public class Account
{
    public int AccountId { get; set; }

    public int UserId { get; set; }

    public string AccountName { get; set; } = string.Empty;

    public string AccountType { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; }
}