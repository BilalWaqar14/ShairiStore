using ShairiStore.Common;
using ShairiStore.Repositories;

namespace ShairiStore.Extensions
{
    public static class NotificationServiceCollectionExtensions
    {
        public static IServiceCollection AddNotificationRepositories(this IServiceCollection services)
        {
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<INotificationStrategy, EmailNotificationStrategy>();
            services.AddScoped<INotificationStrategy, SmsNotificationStrategy>();
            services.AddScoped<NotificationContext>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            return services;
        }
    }
}
