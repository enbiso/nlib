using System;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.EventBus.RabbitMq
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add IntegrationEvent bus wth custom connection and subscription manager
        /// </summary>
        /// <param name="services"></param>        
        /// <param name="option"></param>
        /// <typeparam name="TConnection"></typeparam>
        /// <typeparam name="TEventSubscriptionManager"></typeparam>        
        public static void AddEventBus<TConnection, TEventSubscriptionManager>(this IServiceCollection services, Action<RabbitMqOption> option = null)
            where TConnection : class, IRabbitMqPersistentConnection
            where TEventSubscriptionManager : class, IEventBusSubscriptionsManager
        {
            if(option != null)
                services.Configure(option);

            services.AddSingleton<IRabbitMqPersistentConnection, TConnection>();
            services.AddSingleton<IEventBus, RabbitMqEventBus>();
            services.AddEventBus<TEventSubscriptionManager>();
        }

        /// <summary>
        /// Add IntegrationEvent bus with default connection and manager
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        public static void AddEventBus(this IServiceCollection services, Action<RabbitMqOption> option = null)
        {
            services.AddEventBus<RabbitMqPersistentConnection, EventBusSubscriptionsManager>(option);
        }
    }
}