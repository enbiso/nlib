using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Enbiso.NLib.EventBus
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string eventName, byte[] data);
    }

    public class EventProcessor: IEventProcessor
    {
        private readonly IEventBusSubscriptionsManager _subscriptionsManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;


        public EventProcessor(IEventBusSubscriptionsManager subscriptionsManager, IServiceProvider serviceProvider, ILogger<EventProcessor> logger)
        {
            _subscriptionsManager = subscriptionsManager;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task ProcessEvent(string eventName, byte[] data)
        {
            if (!_subscriptionsManager.HasSubscriptionsForEvent(eventName)) return;

            var message = Encoding.UTF8.GetString(data);

            using (var scope = _serviceProvider.CreateScope())
            {
                var subscriptions = _subscriptionsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        var handler =
                            scope.ServiceProvider.GetService(subscription.HandlerType) as IDynamicEventHandler;
                        var eventData = JsonSerializer.Deserialize<dynamic>(message);
                        if (handler != null) await handler.Handle(eventData);
                        else _logger.LogWarning($"Handler not found for {subscription.HandlerType} on {eventName}.");
                    }
                    else
                    {
                        var eventType = _subscriptionsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonSerializer.Deserialize(message, eventType);
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                        var concreteHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
                        concreteHandlerType.GetMethod("Handle")?.Invoke(handler, new[] { integrationEvent });
                    }
                }
            }
        }
    }
}
