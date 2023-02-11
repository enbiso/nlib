using System;
using System.Linq;
using Enbiso.NLib.EventBus;

namespace Enbiso.NLib.EventInfo;

public interface IEventInfoService
{
    public EventInfoListResponse List();
}

public class EventInfoService: IEventInfoService
{
    private EventInfoListResponse _response;
    
    public EventInfoListResponse List()
    {
        if (_response != null) return _response;
        
        var type = typeof(IEvent);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(p => !(p.FullName?.Contains("NLib") ?? false));

        var types = assemblies
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p));

        var records = types.Select(t => new EventRecord(t)).ToList();
        _response = new EventInfoListResponse
        {
            Records = records,
            Count = records.Count,
        };
        return _response;
    }
}