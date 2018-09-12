using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.EventBus.RabbitMq
{
    public static class ServiceExtensions
    {

        /// <summary>
        /// Add Rabbit MQ with configurations and custom connection and subscription manager
        /// </summary>
        /// <typeparam name="TConnection"></typeparam>
        /// <typeparam name="TEventSubscriptionManager"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddEventBusRabbitMq<TConnection, TEventSubscriptionManager>(this IServiceCollection services,
            IConfiguration configuration)
            where TConnection : class, IRabbitMqPersistentConnection
            where TEventSubscriptionManager : class, IEventBusSubscriptionsManager
        {
            services.AddEventBusRabbitMq<TConnection, TEventSubscriptionManager>(configuration.Bind);
        }

        /// <summary>
        /// Add Event bus wth custom connection and subscription manager
        /// </summary>
        /// <param name="services"></param>        
        /// <param name="option"></param>
        /// <typeparam name="TConnection"></typeparam>
        /// <typeparam name="TEventSubscriptionManager"></typeparam>        
        public static void AddEventBusRabbitMq<TConnection, TEventSubscriptionManager>(this IServiceCollection services, Action<RabbitMqOption> option)
            where TConnection : class, IRabbitMqPersistentConnection
            where TEventSubscriptionManager : class, IEventBusSubscriptionsManager
        {            
            services.Configure(option);

            services.AddSingleton<IRabbitMqPersistentConnection, TConnection>();
            services.AddSingleton<IEventBus, RabbitMqEventBus>();
            services.AddEventBus<TEventSubscriptionManager>();
        }

        /// <summary>
        /// Add Rabbit MQ with configurations
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddEventBusRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEventBusRabbitMq(configuration.Bind);
        }

        /// <summary>
        /// Add Event bus with default connection and manager
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        public static void AddEventBusRabbitMq(this IServiceCollection services, Action<RabbitMqOption> option)
        {
            services.AddEventBusRabbitMq<RabbitMqPersistentConnection, EventBusSubscriptionsManager>(option);
        }
    }
}