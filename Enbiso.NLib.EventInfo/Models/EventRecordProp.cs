using System.Collections.Generic;
using System.Reflection;

namespace Enbiso.NLib.EventInfo.Models;

public class EventRecordProp
{
    public EventRecordProp(PropertyInfo property)
    {
        Name = property.Name;
        Type = property.PropertyType.Name;
    }

    public string Name { get; }
    public string Type { get; }
    public List<EventRecordProp> Props { get; set; }
}