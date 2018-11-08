using System;
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
            services.AddEventBus<EventBusSubscriptionsManager>();
        }

        /// <summary>
        /// Add Event bus with default connection and manager
        /// </summary>
        /// <param name="services"></param>        
        public static void AddEventBus<TEventBusSubscriptionManager>(this IServiceCollection services) where TEventBusSubscriptionManager: class, IEventBusSubscriptionsManager
        {
            services.AddSingleton<IEventBusSubscriptionsManager, TEventBusSubscriptionManager>();
            services.AddSingleton<IEventProcessor, EventProcessor>();
            services.AddSingleton<IEventService, EventService>();
        }

        /// <summary>
        /// Use event bus
        /// </summary>
        /// <param name="app"></param>
        /// <param name="builder"></param>
        public static void UseEventBus(this IApplicationBuilder app, Action<SubscriptionBuilder> builder = null)
        {
            
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            builder?.Invoke(new SubscriptionBuilder(eventBus));
            eventBus.Initialize();
        }
    }

    public class SubscriptionBuilder
    {
        private readonly IEventBus _bus;

        public SubscriptionBuilder(IEventBus bus)
        {
            _bus = bus;
        }

        /// <summary>
        /// Subscribe to events
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        public SubscriptionBuilder Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            _bus.Subscribe<TEvent, TEventHandler>();
            return this;
        }

        /// <summary>
        /// Subscribe to events
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>        
        public SubscriptionBuilder Subscribe<TEvent>()
            where TEvent : IEvent
        {
            _bus.Subscribe<TEvent>();
            return this;
        }

        /// <summary>
        /// Subscribe to events dynamically
        /// </summary>
        /// <param name="eventName"></param>
        /// <typeparam name="TEventHandler"></typeparam>
        public SubscriptionBuilder SubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicEventHandler
        {
            _bus.SubscribeDynamic<TEventHandler>(eventName);
            return this;
        }
    }
}