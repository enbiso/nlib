using System;
using System.Collections.Generic;
using System.Linq;

namespace Enbiso.NLib.EventBus
{
    /// <inheritdoc />
    /// <summary>
    /// In-memory integrationEvent subscription manager
    /// </summary>
    public class EventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        private readonly List<Type> _eventTypes;

        public EventBusSubscriptionsManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        /// <inheritdoc />
        public event EventHandler<string> OnEventRemoved;

        /// <inheritdoc />
        public event EventHandler<string> OnEventAdded;

        /// <inheritdoc />
        public bool IsEmpty => !_handlers.Keys.Any();
        
        /// <inheritdoc />
        public void Clear() => _handlers.Clear();
        
        /// <inheritdoc />
        public void AddDynamicSubscription<TEventHandler>(string eventName) 
            where TEventHandler : IDynamicIntegrationEventHandler
        {
            DoAddSubscription(typeof(TEventHandler), eventName, true);
        }
        
        /// <inheritdoc />
        public void AddSubscription<TEvent, TEventHandler>() where TEvent : IIntegrationEvent 
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = GetEventKey<TEvent>();
            DoAddSubscription(typeof(TEventHandler), eventName, false);
            _eventTypes.Add(typeof(TEvent));
        }

        /// <inheritdoc />
        public void RemoveDynamicSubscription<TEventHandler>(string eventName) 
            where TEventHandler : IDynamicIntegrationEventHandler
        {
            var handlerToRemove = FindDynamicSubscriptionToRemove<TEventHandler>(eventName);
            DoRemoveSubscription(eventName, handlerToRemove);
        }

        /// <inheritdoc />
        public void RemoveSubscription<TEvent, TEventHandler>() 
            where TEventHandler : IEventHandler<TEvent> 
            where TEvent : IIntegrationEvent
        {
            var handlerToRemove = FindSubscriptionToRemove<TEvent, TEventHandler>();
            var eventName = GetEventKey<TEvent>();
            DoRemoveSubscription(eventName, handlerToRemove);
        }

        /// <inheritdoc />
        public bool HasSubscriptionsForEvent<TEvent>() 
            where TEvent : IIntegrationEvent
        {
            var key = GetEventKey<TEvent>();
            return HasSubscriptionsForEvent(key);
        }

        /// <inheritdoc />
        public bool HasSubscriptionsForEvent(string eventName) 
            => _handlers.ContainsKey(eventName);

        /// <inheritdoc />
        public Type GetEventTypeByName(string eventName) 
            => _eventTypes.SingleOrDefault(t => t.Name == eventName);

        /// <inheritdoc />
        public string GetEventKey<TEvent>() 
            where TEvent: IIntegrationEvent
        {
            return typeof(TEvent).Name;
        }

        /// <inheritdoc />
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<TEvent>() 
            where TEvent : IIntegrationEvent
        {
            var key = GetEventKey<TEvent>();
            return GetHandlersForEvent(key);
        }
        
        /// <inheritdoc />
        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) 
            => _handlers[eventName];

        
        #region private methods

        private void DoRemoveSubscription(string eventName, SubscriptionInfo subsToRemove)
        {
            if (subsToRemove == null) return;

            _handlers[eventName].Remove(subsToRemove);

            if (_handlers[eventName].Any()) return;

            _handlers.Remove(eventName);
            var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
            if (eventType != null)
            {
                _eventTypes.Remove(eventType);
            }
            RaiseOnEventRemoved(eventName);
        }

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            if (handler != null)
            {
                OnEventRemoved?.Invoke(this, eventName);
            }
        }
        
        private void RaiseOnEventAdded(string eventName)
        {
            var handler = OnEventAdded;
            if (handler != null)
            {
                OnEventAdded?.Invoke(this, eventName);
            }
        }

        private SubscriptionInfo FindDynamicSubscriptionToRemove<TEventHandler>(string eventName)
            where TEventHandler : IDynamicIntegrationEventHandler
        {
            return DoFindSubscriptionToRemove(eventName, typeof(TEventHandler));
        }

        private SubscriptionInfo FindSubscriptionToRemove<TEvent, TEventHandler>()
            where TEvent : IIntegrationEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = GetEventKey<TEvent>();
            return DoFindSubscriptionToRemove(eventName, typeof(TEventHandler));
        }

        private SubscriptionInfo DoFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            return !HasSubscriptionsForEvent(eventName) ? null : _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
        }

        private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }

            if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName].Add(isDynamic ? SubscriptionInfo.Dynamic(handlerType) : SubscriptionInfo.Typed(handlerType));
            RaiseOnEventAdded(eventName);

        }
        
        #endregion
        
    }
}
