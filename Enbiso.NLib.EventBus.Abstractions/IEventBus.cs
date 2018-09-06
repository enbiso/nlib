namespace Enbiso.NLib.EventBus
{
    /// <summary>
    /// IntegrationEvent bus interface
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Initialize eventbus with subscriptions        
        /// </summary>        
        void Initialize();

        /// <summary>
        /// Publish integrationEvent
        /// </summary>
        /// <param name="integrationEvent"></param>
        void Publish(IIntegrationEvent integrationEvent);

        /// <summary>
        /// Subscribe to events
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IIntegrationEvent
            where TEventHandler : IEventHandler<TEvent>;

        /// <summary>
        /// Subscribe to events
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>        
        void Subscribe<TEvent>()
            where TEvent : IIntegrationEvent;

        /// <summary>
        /// Subscribe to events dynamically
        /// </summary>
        /// <param name="eventName"></param>
        /// <typeparam name="TEventHandler"></typeparam>
        void SubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicIntegrationEventHandler;

        /// <summary>
        /// Unsusbcribe to integrationEvent dynamically
        /// </summary>
        /// <param name="eventName"></param>
        /// <typeparam name="TEventHandler"></typeparam>
        void UnsubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicIntegrationEventHandler;

        /// <summary>
        /// Unsubscribe to events
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        void Unsubscribe<TEvent, TEventHandler>()
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : IIntegrationEvent;
    }
}
