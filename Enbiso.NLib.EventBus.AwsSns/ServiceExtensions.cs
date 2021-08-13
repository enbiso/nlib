using System;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.EventBus.AwsSns
{
    public static class ServiceExtensions
    {
        /// <summary>
        /// Add Event bus with default connection and manager
        /// </summary>
        /// <param name="services"></param>
        /// <param name="option"></param>
        public static void AddEventBusAwsSns(this IServiceCollection services, Action<AwsSnsOptions> option)
        {
            services.Configure(option);

            services.AddEventBus();
            services.AddSingleton<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>();
            services.AddSingleton<IEventPublisher, AwsSnsEventPublisher>();
            services.AddSingleton<IEventSubscriber, AwsSnsEventSubscriber>();
        }
    }
}