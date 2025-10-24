using ShairiStore.Common;
using ShairiStore.Repositories;
using ShairiStore.Services;

namespace ShairiStore.Extensions;

public static class ExportServiceExtensions
{
    public static IServiceCollection AddHelperSerivces(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IExportService, ExportService>();

        return services;
    }
}
