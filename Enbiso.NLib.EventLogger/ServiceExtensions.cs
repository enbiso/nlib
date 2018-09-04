using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.EventLogger
{
    public static class ServiceExtensions
    {
        public static void AddEventLogger(this IServiceCollection services)
        {
            services.AddTransient<IEventLoggerService, EventLoggerService>();
        }
    }
}