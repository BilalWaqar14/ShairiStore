namespace ShairiStore.Common;

public class NotificationContext
{
    private readonly IEnumerable<INotificationStrategy> _strategies;

    public NotificationContext(IEnumerable<INotificationStrategy> strategies)
    {
        _strategies = strategies;
    }

    public async Task NotifyAllAsync(string to, string subject, string message)
    {
        foreach (var strategy in _strategies)
        {
            await strategy.SendNotificationAsync(to, subject, message);
        }
    }
}
