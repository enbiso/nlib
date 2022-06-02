using System;
using Enbiso.NLib.EventBus.Nats.JetStream;
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

            var opts = new NatsOptions();
            option.Invoke(opts);
            
            if (opts.EnableJetStream)
            {
                services.AddSingleton<IJetStreamConnection, JetStreamConnection>();
                services.AddSingleton<IEventPublisher, JetStreamEventPublisher>();
                services.AddSingleton<IEventSubscriber, JetStreamEventSubscriber>();
            }
            else
            {
                services.AddSingleton<IEventPublisher, NatsEventPublisher>();
                services.AddSingleton<IEventSubscriber, NatsEventSubscriber>();
            }
        }
    }
}