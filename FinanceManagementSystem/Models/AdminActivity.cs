namespace FinanceManagementSystem.Models;

public class AdminActivity
{
    public int AdminActivityId { get; set; }

    public int AdminUserId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public int? EntityId { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}