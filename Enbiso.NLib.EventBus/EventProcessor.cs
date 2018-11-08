using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Enbiso.NLib.EventBus
{
    public interface IEventProcessor
    {
        Task ProcessEvent(string eventName, string message);
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

        public async Task ProcessEvent(string eventName, string message)
        {
            if (!_subscriptionsManager.HasSubscriptionsForEvent(eventName)) return;

            using (var scope = _serviceProvider.CreateScope())
            {
                var subscriptions = _subscriptionsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        var handler =
                            scope.ServiceProvider.GetService(subscription.HandlerType) as IDynamicEventHandler;
                        dynamic eventData = JObject.Parse(message);
                        if (handler != null) await handler.Handle(eventData);
                        else _logger.LogWarning($"Handler not found for {subscription.HandlerType} on {eventName}.");
                    }
                    else
                    {
                        var eventType = _subscriptionsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
                        var concreteHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteHandlerType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });
                    }
                }
            }
        }
    }
}
