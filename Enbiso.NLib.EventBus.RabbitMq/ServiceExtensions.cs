using Enbiso.NLib.EventBus.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.EventBus.RabbitMq
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add Event bus wth custom connection and subscription manager
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        /// <typeparam name="TConnection"></typeparam>
        /// <typeparam name="TEventSubscriptionManager"></typeparam>        
        public static void AddEventBus<TConnection, TEventSubscriptionManager>(this IServiceCollection services, RabbitMqOption option)
            where TConnection: class, IRabbitMqPersistentConnection
            where TEventSubscriptionManager: class, IEventBusSubscriptionsManager
        {
            services.AddSingleton(option);
            services.AddSingleton<IRabbitMqPersistentConnection, TConnection>();
            services.AddSingleton<IEventBus, RabbitMqEventBus>();
            services.AddSingleton<IEventBusSubscriptionsManager, TEventSubscriptionManager>();
        }
        
        /// <summary>
        /// Add Event bus with default connection and manager
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        public static void AddEventBus(this IServiceCollection services, RabbitMqOption option)
        {
            services.AddEventBus<DefaultRabbitMqPersistentConnection, InMemoryEventBusSubscriptionsManager>(option);
        }
    }
}