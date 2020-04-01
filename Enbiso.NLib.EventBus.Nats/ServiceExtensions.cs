using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NATS.Client;

namespace Enbiso.NLib.EventBus.Nats
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add Event bus with default connection and manager
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        public static void AddEventBusNats(this IServiceCollection services, Action<NatsOptions> option)
        {
            services.Configure(option);

            services.AddEventBus();
            services.AddSingleton<ConnectionFactory>();
            services.AddSingleton<INatsConnection, NatsConnection>();
            services.AddSingleton<IEventPublisher, NatsEventPublisher>();
            services.AddSingleton<IEventSubscriber, NatsEventSubscriber>();
        }
    }
}