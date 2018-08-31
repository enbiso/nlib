using System;
using System.Collections.Generic;

namespace Enbiso.NLib.EventBus.Abstractions
{
    /// <summary>
    /// Event subscription manager interface
    /// </summary>
    public interface IEventBusSubscriptionsManager
    {
        /// <summary>
        /// Is subscription empty
        /// </summary>
        bool IsEmpty { get; }
        
        /// <summary>
        /// Event removed event handler
        /// </summary>
        event EventHandler<string> OnEventRemoved;
        
        /// <summary>
        /// Event added event handler
        /// </summary>
        event EventHandler<string> OnEventAdded;
        
        /// <summary>
        /// Add dynamic subscription to manager
        /// </summary>
        /// <param name="eventName"></param>
        /// <typeparam name="TEventHandler"></typeparam>
        void AddDynamicSubscription<TEventHandler>(string eventName) 
            where TEventHandler : IDynamicIntegrationEventHandler;

        /// <summary>
        /// Remove dynamic event subscription
        /// </summary>
        /// <param name="eventName"></param>
        /// <typeparam name="TEventHandler"></typeparam>
        void RemoveDynamicSubscription<TEventHandler>(string eventName) 
            where TEventHandler : IDynamicIntegrationEventHandler;
        
        /// <summary>
        /// Check if subscription exist for dynamic events
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        bool HasSubscriptionsForEvent(string eventName);
        
        /// <summary>
        /// Get event type for dynamic events
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Type GetEventTypeByName(string eventName);
        
        /// <summary>
        /// Add subscription to manager
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        void AddSubscription<TEvent, TEventHandler>() 
            where TEvent : IIntegrationEvent 
            where TEventHandler : IIntegrationEventHandler<TEvent>;
        
        /// <summary>
        /// Remove subscription
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="TEventHandler"></typeparam>
        void RemoveSubscription<TEvent, TEventHandler>() 
            where TEventHandler : IIntegrationEventHandler<TEvent> 
            where TEvent : IIntegrationEvent;

        /// <summary>
        /// Check if a subsciption exists for an event
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        bool HasSubscriptionsForEvent<TEvent>() 
            where TEvent : IIntegrationEvent;

        /// <summary>
        /// Clear all event handlers
        /// </summary>
        void Clear();
        
        /// <summary>
        /// Get all event subscriptions given the event
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<TEvent>() 
            where TEvent : IIntegrationEvent;
        
        /// <summary>
        /// Get all event subscrptions given the dynamic event
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        
        /// <summary>
        /// Get event key for given event
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        string GetEventKey<TEvent>() 
            where TEvent : IIntegrationEvent;
    }
}