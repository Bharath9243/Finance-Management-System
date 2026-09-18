using FinanceManagementSystem.Models;

namespace FinanceManagementSystem.Services.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<Notification>> GetMyNotificationsAsync(
        int userId);

    Task MarkAsReadAsync(
        int userId,
        int notificationId);
}