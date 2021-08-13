using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.EventBus
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add Event bus with default connection and manager
        /// </summary>
        /// <param name="services"></param>        
        public static void AddEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventProcessor, EventProcessor>();
            services.AddSingleton<IEventService, EventService>();
            services.AddTransient<IEventSubscriptionService, EventSubscriptionService>();
        }
    }
}