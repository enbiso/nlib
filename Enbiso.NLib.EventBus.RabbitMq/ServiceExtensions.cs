using System;
using Microsoft.Extensions.Configuration;
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
        public static void AddEventBusRabbitMq(this IServiceCollection services, Action<RabbitMqOption> option)
        {            
            services.Configure(option);

            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
            services.AddSingleton<IEventSubscriber, RabbitMqEventSubscriber>();
        }
    }
}