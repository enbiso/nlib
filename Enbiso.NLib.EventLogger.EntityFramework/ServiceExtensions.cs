using System.Linq;
using Enbiso.NLib.EventBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.EventLogger.EntityFramework
{
    public static class ServiceExtensions
    {
        public static void AddEventLogger<TDbContext>(this IServiceCollection services) where TDbContext: DbContext
        {
            services.AddEventLogger();
            services.AddSingleton<IEventLogRepo, EntityEventLogRepo<TDbContext>>();

            var eventService = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IEventService));
            if (eventService != null) services.Remove(eventService);

            services.AddSingleton<IEventService, EntityEventLoggerEventService<TDbContext>>();
        }
    }
}