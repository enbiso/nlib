using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.EventInfo;

public static class ServiceExtensions
{
    public static IServiceCollection AddEventInfo(this IServiceCollection collection, Action<EventInfoOption> optBuilder = null)
    {
        collection.AddOptions();
        optBuilder ??= _ => {};
        collection.Configure(optBuilder);
        
        collection.AddSingleton<IEventInfoService, EventInfoService>();
        return collection;
    }
}

public class EventInfoOption
{
    public List<Type> SearchTypes { get; set; } = new();
}