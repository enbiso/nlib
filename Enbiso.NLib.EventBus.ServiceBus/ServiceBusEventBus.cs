using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Enbiso.NLib.EventBus.Abstractions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Enbiso.NLib.EventBus.ServiceBus
{
    /// <summary>
    /// Eventbus Service bus
    /// </summary>
    /// /// <inheritdoc />
    public class ServiceBusEventBus : IEventBus
    {
        private const string AutofacScopeName = "scope_service_bus";
        private const string IntegrationEventSufix = "IntegrationEvent";

        private readonly IServiceBusPersisterConnection _serviceBusPersisterConnection;
        private readonly ILogger<ServiceBusEventBus> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly SubscriptionClient _subscriptionClient;
        private readonly ILifetimeScope _autofac;
        private readonly IEnumerable<IEventBusSubscriber> _subscribers;

        /// <summary>
        /// Constrsuctor
        /// </summary>
        /// <param name="serviceBusPersisterConnection"></param>
        /// <param name="logger"></param>
        /// <param name="subsManager"></param>        
        /// <param name="subscriptionClientName"></param>
        /// <param name="autofac"></param>
        public ServiceBusEventBus(IServiceBusPersisterConnection serviceBusPersisterConnection,
            ILogger<ServiceBusEventBus> logger, IEventBusSubscriptionsManager subsManager,            
            string subscriptionClientName,
            ILifetimeScope autofac, 
            IEnumerable<IEventBusSubscriber> subscribers)
        {
            _serviceBusPersisterConnection = serviceBusPersisterConnection;
            _logger = logger;
            _subsManager = subsManager ?? new InMemoryEventBusSubscriptionsManager();

            _subscriptionClient = new SubscriptionClient(
                serviceBusPersisterConnection.ServiceBusConnectionStringBuilder,
                subscriptionClientName);
            _autofac = autofac;            

            RemoveDefaultRule();
            _subscribers = subscribers;
        }

        /// <inheritdoc />
        public void Initialize()
        {
            _subscribers.Subscribe();
            RegisterSubscriptionClientMessageHandler();
        }

        /// <inheritdoc />
        public void Publish(IIntegrationEvent @event)
        {
            var eventName = @event.GetType().Name.Replace(IntegrationEventSufix, "");
            var jsonMessage = JsonConvert.SerializeObject(@event);

            var message = new Message
            {
                MessageId = new Guid().ToString(),
                Body = Encoding.UTF8.GetBytes(jsonMessage),
                Label = eventName,
            };

            var topicClient = _serviceBusPersisterConnection.CreateModel();

            topicClient.SendAsync(message)
                .GetAwaiter()
                .GetResult();
        }

        /// <inheritdoc />
        public void Subscribe<TEvent>() where TEvent : IIntegrationEvent
        {
            Subscribe<TEvent, IIntegrationEventHandler<TEvent>>();
        }

        /// <inheritdoc />
        public void SubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicIntegrationEventHandler
        {
            _subsManager.AddDynamicSubscription<TEventHandler>(eventName);
        }

        /// <inheritdoc />
        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : IIntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name.Replace(IntegrationEventSufix, "");

            var containsKey = _subsManager.HasSubscriptionsForEvent<TEvent>();
            if (!containsKey)
            {
                try
                {
                    _subscriptionClient.AddRuleAsync(new RuleDescription
                    {
                        Filter = new CorrelationFilter {Label = eventName},
                        Name = eventName
                    }).GetAwaiter().GetResult();
                }
                catch (ServiceBusException)
                {
                    _logger.LogInformation($"The messaging entity {eventName} already exists.");
                }
            }

            _subsManager.AddSubscription<TEvent, TEventHandler>();
        }

        /// <inheritdoc />
        public void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : IIntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name.Replace(IntegrationEventSufix, "");

            try
            {
                _subscriptionClient
                    .RemoveRuleAsync(eventName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogInformation($"The messaging entity {eventName} Could not be found.");
            }

            _subsManager.RemoveSubscription<TEvent, TEventHandler>();
        }

        /// <inheritdoc />
        public void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
        }
        
        public void Dispose()
        {
            _subsManager.Clear();
        }

        #region private methods

        private void RegisterSubscriptionClientMessageHandler()
        {
            _subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var eventName = $"{message.Label}{IntegrationEventSufix}";
                    var messageData = Encoding.UTF8.GetString(message.Body);
                    await ProcessEvent(eventName, messageData);

                    // Complete the message so that it is not received again.
                    await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                },
                new MessageHandlerOptions(ExceptionReceivedHandler) {MaxConcurrentCalls = 10, AutoComplete = false});
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _autofac.BeginLifetimeScope(AutofacScopeName))
                {
                    var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        if (subscription.IsDynamic)
                        {
                            var handler =
                                scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                            dynamic eventData = JObject.Parse(message);
                            if (handler != null) await handler.Handle(eventData);
                            else _logger.LogWarning($"Handler not found for {subscription.HandlerType} on {eventName}.");
                        }
                        else
                        {
                            var eventType = _subsManager.GetEventTypeByName(eventName);
                            var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                            var handler = scope.ResolveOptional(subscription.HandlerType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                            await (Task) concreteType.GetMethod("Handle")
                                .Invoke(handler, new[] {integrationEvent});
                        }
                    }
                }
            }
        }

        private void RemoveDefaultRule()
        {
            try
            {
                _subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogInformation($"The messaging entity {RuleDescription.DefaultRuleName} Could not be found.");
            }
        }

        #endregion
        
    }
}
