using Microsoft.Extensions.DependencyInjection;

namespace Enbiso.NLib.EventInfo;

public static class EventInfoServiceExtensions
{
    public static IServiceCollection AddEventInfo(this IServiceCollection collection)
    {
        collection.AddSingleton<IEventInfoService, EventInfoService>();
        return collection;
    }
}