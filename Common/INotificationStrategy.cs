namespace ShairiStore.Common;

public interface INotificationStrategy
{
    Task SendNotificationAsync(string to, string subject, string message);
}
