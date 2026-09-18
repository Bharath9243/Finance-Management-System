using FinanceManagementSystem.Models;
using System.Data;

namespace FinanceManagementSystem.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<int> CreateAsync(
        Notification notification,
        IDbTransaction dbTransaction);

    Task<IEnumerable<Notification>> GetByUserIdAsync(
        int userId);

    Task MarkAsReadAsync(
        int notificationId,
        int userId);
}