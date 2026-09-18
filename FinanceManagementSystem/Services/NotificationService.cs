using FinanceManagementSystem.Models;
using FinanceManagementSystem.Repositories.Interfaces;
using FinanceManagementSystem.Services.Interfaces;

namespace FinanceManagementSystem.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(
        INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<Notification>>
        GetMyNotificationsAsync(int userId)
    {
        return await _notificationRepository
            .GetByUserIdAsync(userId);
    }

    public async Task MarkAsReadAsync(
        int userId,
        int notificationId)
    {
        await _notificationRepository
            .MarkAsReadAsync(
                notificationId,
                userId);
    }
}