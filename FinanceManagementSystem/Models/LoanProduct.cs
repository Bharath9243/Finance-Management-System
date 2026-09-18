namespace FinanceManagementSystem.Models;

public class LoanProduct
{
    public int LoanProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal InterestRate { get; set; }

    public string InterestType { get; set; } = "Simple";

    public decimal MaxAmount { get; set; }

    public int MaxTenureMonths { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
}