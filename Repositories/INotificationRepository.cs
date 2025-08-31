using ShairiStore.Models;

namespace ShairiStore.Repositories;

public interface INotificationRepository
{
    Task<NotificationDetails> CreateNotificationAsync(NotificationDetails notification);
    Task<IEnumerable<NotificationDetails>> ListNotificationsAsync(string userId);
    Task<NotificationDetails?> GetNotificationByIdAsync(int notificationId);
    Task<bool> SetNotificationIsReadAsync(int notificationId);
    Task<bool> DeleteNotificationAsync(int notificationId);
}
