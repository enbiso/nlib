using System;
using System.Collections.Generic;
using System.Linq;

namespace Enbiso.NLib.EventInfo.Models;

public class EventRecord
{
    public string Name { get; }
    public List<EventRecordProp> Props { get; }

    public EventRecord(Type type)
    {
        Name = type.Name;
        Props = type.GetProperties().Select(EventRecordMapper.Map).ToList();
    }
}