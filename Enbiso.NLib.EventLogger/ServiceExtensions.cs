using System.Linq;
using Enbiso.NLib.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.EventLogger
{
    public static class ServiceExtensions
    {
        public static void AddEventLogger(this IServiceCollection services)
        {
            services.AddTransient<IEventLoggerService, EventLoggerService>();

            var eventService = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IEventService));
            if (eventService != null) services.Remove(eventService);

            services.AddSingleton<IEventService, EventLoggerEventService>();
        }
    }
}