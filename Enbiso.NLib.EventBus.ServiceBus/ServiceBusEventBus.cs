using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Enbiso.NLib.EventBus.ServiceBus
{
    /// <summary>
    /// Event bus Service bus
    /// </summary>
    /// /// <inheritdoc />
    public class ServiceBusEventBus : IEventBus
    {
        private const string IntegrationEventSuffix = "Event";

        private readonly IServiceBusPersistenceConnection _serviceBusPersistenceConnection;
        private readonly ILogger<ServiceBusEventBus> _logger;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private readonly SubscriptionClient _subscriptionClient;
        private readonly IEventProcessor _eventProcessor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceBusPersistenceConnection"></param>
        /// <param name="logger"></param>
        /// <param name="subsManager"></param>        
        /// <param name="subscriptionClientName"></param>
        /// <param name="eventProcessor"></param>        
        public ServiceBusEventBus(IServiceBusPersistenceConnection serviceBusPersistenceConnection,
            ILogger<ServiceBusEventBus> logger, IEventBusSubscriptionsManager subsManager,            
            string subscriptionClientName, IEventProcessor eventProcessor)
        {
            _serviceBusPersistenceConnection = serviceBusPersistenceConnection;
            _logger = logger;
            _eventProcessor = eventProcessor;
            _subsManager = subsManager ?? new EventBusSubscriptionsManager();

            _subscriptionClient = new SubscriptionClient(
                serviceBusPersistenceConnection.ServiceBusConnectionStringBuilder,
                subscriptionClientName);                     

            RemoveDefaultRule();
        }

        /// <inheritdoc />
        public void Initialize()
        {
            RegisterSubscriptionClientMessageHandler();
        }

        /// <inheritdoc />
        public void Publish(IEvent @event, string exchange = null)
        {            
            var eventName = @event.GetType().Name.Replace(IntegrationEventSuffix, "");
            var jsonMessage = JsonSerializer.Serialize(@event);

            var message = new Message
            {
                MessageId = new Guid().ToString(),
                Body = Encoding.UTF8.GetBytes(jsonMessage),
                Label = eventName,
            };

            var topicClient = _serviceBusPersistenceConnection.CreateModel();

            topicClient.SendAsync(message)
                .GetAwaiter()
                .GetResult();            
        }

        /// <inheritdoc />
        public void Subscribe<TEvent>() where TEvent : IEvent
        {
            Subscribe<TEvent, IEventHandler<TEvent>>();
        }

        /// <inheritdoc />
        public void SubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicEventHandler
        {
            _subsManager.AddDynamicSubscription<TEventHandler>(eventName);
        }

        /// <inheritdoc />
        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name.Replace(IntegrationEventSuffix, "");

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
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name.Replace(IntegrationEventSuffix, "");

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
            where TH : IDynamicEventHandler
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
                    var eventName = $"{message.Label}{IntegrationEventSuffix}";                    
                    await _eventProcessor.ProcessEvent(eventName, message.Body);

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
