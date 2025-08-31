namespace ShairiStore.Common;

public class SmsNotificationStrategy : INotificationStrategy
{
    private readonly IConfiguration _config;

    public SmsNotificationStrategy(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendNotificationAsync(string to, string subject, string message)
    {
        // Read SMS Provider settings from appsettings.json
        var smsProvider = _config["NotificationSettings:Sms:Provider"];
        var smsApiKey = _config["NotificationSettings:Sms:ApiKey"];

        // TODO: Later implement Twilio, Nexmo, etc.
        // For now, just simulate
        await Task.Run(() =>
        {
            Console.WriteLine($"SMS sent to {to} via {smsProvider}: {message}");
        });
    }
}
