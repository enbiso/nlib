using Microsoft.AspNetCore.Builder;
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
        }

        /// <summary>
        /// Use event bus
        /// </summary>
        /// <param name="app"></param>
        public static void UseEventBus(this IApplicationBuilder app)
        {
            
            var eventSubscriber = app.ApplicationServices.GetRequiredService<IEventSubscriber>();
            eventSubscriber.Initialize();
        }
    }
}