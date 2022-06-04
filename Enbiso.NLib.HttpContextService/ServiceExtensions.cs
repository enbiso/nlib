using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.HttpContextService;

public static class ServiceExtensions
{
    public static void AddHttpContextService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IHttpContextFetcher, HttpContextFetcher>();
        services.AddSingleton<IHttpContextService, HttpContextService>();
    }
}