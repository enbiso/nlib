namespace Enbiso.NLib.EventBus
{
    /// <summary>
    /// Event bus interface
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Initialize eventbus with subscriptions        
        /// </summary>        
        void Initialize();

        /// <summary>
        /// Publish @event
        /// </summary>
        /// <param name="event"></param>
        /// <param name="exchange"></param>
        void Publish(IEvent @event, string exchange = null);

        /// <summary>
        /// Subscribe to events
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IEvent
            where TEventHandler : IEventHandler<TEvent>;

        /// <summary>
        /// Subscribe to events
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>        
        void Subscribe<TEvent>()
            where TEvent : IEvent;

        /// <summary>
        /// Subscribe to events dynamically
        /// </summary>
        /// <param name="eventName"></param>
        /// <typeparam name="TEventHandler"></typeparam>
        void SubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicEventHandler;

        /// <summary>
        /// Unsusbcribe to @event dynamically
        /// </summary>
        /// <param name="eventName"></param>
        /// <typeparam name="TEventHandler"></typeparam>
        void UnsubscribeDynamic<TEventHandler>(string eventName)
            where TEventHandler : IDynamicEventHandler;

        /// <summary>
        /// Unsubscribe to events
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        void Unsubscribe<TEvent, TEventHandler>()
            where TEventHandler : IEventHandler<TEvent>
            where TEvent : IEvent;
    }
}
