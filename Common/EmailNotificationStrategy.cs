namespace ShairiStore.Common;

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailNotificationStrategy : INotificationStrategy
{
    private readonly IConfiguration _config;

    public EmailNotificationStrategy(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendNotificationAsync(string to, string subject, string message)
    {
        var smtpSection = _config.GetSection("NotificationSettings:Email");
        var smtpHost = smtpSection["Host"];
        var smtpPort = int.Parse(smtpSection["Port"]);
        var smtpUser = smtpSection["Username"];
        var smtpPass = smtpSection["Password"];

        using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
        {
            smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPass);
            smtpClient.EnableSsl = true;

            var mailMessage = new MailMessage(smtpUser, to, subject, message);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
