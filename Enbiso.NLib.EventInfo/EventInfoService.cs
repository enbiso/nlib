using System;
using System.Collections.Generic;
using System.Linq;
using Enbiso.NLib.EventBus;
using Enbiso.NLib.EventInfo.Models;
using Microsoft.Extensions.Options;

namespace Enbiso.NLib.EventInfo;

public interface IEventInfoService
{
    public EventInfoListResponse List();
}

public class EventInfoService: IEventInfoService
{
    private EventInfoListResponse _response;
    private readonly EventInfoOption _option;

    public EventInfoService(IOptions<EventInfoOption> option)
    {
        _option = option.Value;
    }

    public EventInfoListResponse List()
    {
        if (_response != null) return _response;

        var parentTypes = _option.SearchTypes ?? new List<Type>();
        parentTypes.Add(typeof(IEvent));

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(p => !(p.FullName?.Contains("NLib") ?? false));

        var types = assemblies
            .SelectMany(s => s.GetTypes())
            .Where(t => t.IsClass)
            .Where(t => parentTypes.Any(pt => pt.IsAssignableFrom(t)));

        var records = types.Select(t => new EventRecord(t)).ToList();
        _response = new EventInfoListResponse
        {
            Records = records,
            Count = records.Count,
        };
        return _response;
    }
}