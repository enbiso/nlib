using System;
using System.Collections.Generic;

namespace Enbiso.NLib.EventBus;

public interface IEventTypeManager
{
    public void Add(Type eventType);
    public void OnEventTypeAdded(EventTypeAddedEventHandler handler);
}

public class EventTypeManager: IEventTypeManager
{
    private readonly List<Type> _eventTypes = new();
    private event EventTypeAddedEventHandler EventTypeAdded;
    
    public void Add(Type eventType)
    {
        if (_eventTypes.Contains(eventType)) return;
        _eventTypes.Add(eventType);
        EventTypeAdded?.Invoke(this, new EventTypeAddedEventArgs(eventType));
    }

    public void OnEventTypeAdded(EventTypeAddedEventHandler handler)
    {
        ReplayEventsForHandler(handler);
        EventTypeAdded += handler;
    }

    private void ReplayEventsForHandler(EventTypeAddedEventHandler handler)
    {
        foreach (var eventType in _eventTypes) 
            handler(this, new EventTypeAddedEventArgs(eventType));
    }
}

    
public class EventTypeAddedEventArgs: EventArgs
{
    public EventTypeAddedEventArgs(Type eventType)
    {
        EventType = eventType;
    }

    public Type EventType { get; }
}
    
public delegate void EventTypeAddedEventHandler(object sender, EventTypeAddedEventArgs e);