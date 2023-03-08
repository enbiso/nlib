using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Enbiso.NLib.EventInfo.Models;

public static class EventRecordMapper
{
    public static EventRecordProp Map(PropertyInfo property)
    {
        var prop = new EventRecordProp(property);
        if (!IsSimpleType(property.PropertyType))
            prop.Props = property.PropertyType.GetProperties()
                .Select(p => new EventRecordProp(p)).ToList();
        return prop;
    }

    private static bool ShouldExtract(Type type)
    {
        if (IsSimpleType(type)) return false;
        var excludeTypes = new List<Type>
        {
            typeof(List<>),
            typeof(IEnumerable<>),
            typeof(Dictionary<,>),
            typeof(IDictionary<,>)
        };
        return excludeTypes.All(p => p == type);
    }

    private static bool IsSimpleType(Type type)
    {
        return
            type.IsPrimitive ||
            type.IsValueType ||
            new[]
            {
                typeof(string),
                typeof(decimal),
                typeof(DateTime),
                typeof(DateTimeOffset),
                typeof(TimeSpan),
                typeof(Guid)
            }.Contains(type) ||
            type.IsEnum ||
            Convert.GetTypeCode(type) != TypeCode.Object ||
            (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
             IsSimpleType(type.GetGenericArguments()[0]));
    }
}