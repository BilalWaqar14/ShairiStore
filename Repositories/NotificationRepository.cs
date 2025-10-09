using Microsoft.EntityFrameworkCore;
using ShairiStore.Common;
using ShairiStore.Models;

namespace ShairiStore.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    private readonly NotificationContext _notificationContext;

    public NotificationRepository(AppDbContext context, NotificationContext notificationContext)
    {
        _context = context;
        _notificationContext = notificationContext;
    }

    public async Task<NotificationDetails> CreateNotificationAsync(NotificationDetails notification)
    {
        notification.NotificationCreatedAt = DateTime.UtcNow;
        notification.IsRead = false;
        notification.IsActive = true;

        _context.NotificationDetails.Add(notification);
        await _context.SaveChangesAsync();

        //// Send Email + SMS to Super Admin
        //string superAdminEmail = "superadmin@yourdomain.com";  // Replace with config lookup
        //string superAdminPhone = "+92000000000";              // Replace with config lookup

        //await _notificationContext.NotifyAllAsync(
        //    superAdminEmail,
        //    "New Notification",
        //    notification.NotificationContent
        //);

        return notification;
    }

    public async Task<IEnumerable<NotificationDetails>> ListNotificationsAsync(string module)
    {
        return await _context.NotificationDetails.Where(x=> x.NotificationModule == module)
            .Include(n => n.NotificationType)
            .Include(n => n.User)
            .OrderByDescending(n => n.NotificationCreatedAt)
            .ToListAsync();
    }

    public async Task<NotificationDetails?> GetNotificationByIdAsync(int notificationId)
    {
        return await _context.NotificationDetails
            .Include(n => n.NotificationType)
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.IsActive);
    }

    public async Task<bool> SetNotificationIsReadAsync(int notificationId)
    {
        var notification = await _context.NotificationDetails.FindAsync(notificationId);
        if (notification == null) return false;

        notification.IsRead = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteNotificationAsync(int notificationId)
    {
        var notification = await _context.NotificationDetails.FindAsync(notificationId);
        if (notification == null) return false;

        notification.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
