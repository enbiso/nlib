using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;

namespace Enbiso.NLib.EventBus.Nats
{
    public static class ServiceExtensions
    {

        /// <summary>
        /// Add NATs with configurations and custom connection and subscription manager
        /// </summary>
        /// <typeparam name="TConnection"></typeparam>
        /// <typeparam name="TEventSubscriptionManager"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddEventBusNats<TConnection, TEventSubscriptionManager>(this IServiceCollection services,
            IConfiguration configuration)
            where TConnection : class, INatsPersistentConnection
            where TEventSubscriptionManager : class, IEventBusSubscriptionsManager
        {
            services.AddEventBusNats<TConnection, TEventSubscriptionManager>(configuration.Bind);
        }

        /// <summary>
        /// Add Event bus wth custom connection and subscription manager
        /// </summary>
        /// <param name="services"></param>        
        /// <param name="option"></param>
        /// <typeparam name="TConnection"></typeparam>
        /// <typeparam name="TEventSubscriptionManager"></typeparam>        
        public static void AddEventBusNats<TConnection, TEventSubscriptionManager>(this IServiceCollection services, Action<NatsOptions> option)
            where TConnection : class, INatsPersistentConnection
            where TEventSubscriptionManager : class, IEventBusSubscriptionsManager
        {            
            services.Configure(option);

            services.AddSingleton<ConnectionFactory>();
            services.AddSingleton<INatsPersistentConnection, TConnection>();
            services.AddSingleton<IEventBus, NatsEventBus>();
            services.AddEventBus<TEventSubscriptionManager>();
        }

        /// <summary>
        /// Add Rabbit MQ with configurations
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddEventBusNats(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEventBusNats(configuration.Bind);
        }

        /// <summary>
        /// Add Event bus with default connection and manager
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        public static void AddEventBusNats(this IServiceCollection services, Action<NatsOptions> option)
        {
            services.AddEventBusNats<NatsPersistentConnection, EventBusSubscriptionsManager>(option);
        }
    }
}