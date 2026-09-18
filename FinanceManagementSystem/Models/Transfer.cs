namespace FinanceManagementSystem.Models;

public class Transfer
{
    public int TransferId { get; set; }

    public int FromAccountId { get; set; }

    public int ToAccountId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransferDate { get; set; }
}