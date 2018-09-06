using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.EventBus
{
    public static class ServiceExtensions
    {      
        /// <summary>
        /// Add IntegrationEvent bus with default connection and manager
        /// </summary>
        /// <param name="services"></param>        
        public static void AddEventBus(this IServiceCollection services)
        {
            services.AddEventBus<EventBusSubscriptionsManager>();
        }

        /// <summary>
        /// Add IntegrationEvent bus with default connection and manager
        /// </summary>
        /// <param name="services"></param>        
        public static void AddEventBus<TEventBusSubscriptionManager>(this IServiceCollection services) where TEventBusSubscriptionManager: class, IEventBusSubscriptionsManager
        {
            services.AddSingleton<IEventBusSubscriptionsManager, TEventBusSubscriptionManager>();
        }

        /// <summary>
        /// Use event bus
        /// </summary>
        /// <param name="app"></param>
        public static void UseEventBus(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Initialize();
        }
    }
}