using System;
using System.Collections.Generic;
using System.Linq;

namespace Enbiso.NLib.EventInfo;

public class EventInfoListResponse
{
    public List<EventRecord> Records { get; set; } = new();
    public int Count { get; set; }
}

public class EventRecord
{
    public EventRecord(Type type)
    {
        Name = type.Name;
        Props = type.GetProperties().Select(p => new EventRecordProp(p.Name, p.PropertyType.Name)).ToList();
    }

    public string Name { get; set; }
    public List<EventRecordProp> Props { get; set; }
}

public record EventRecordProp(string Name, string Type);