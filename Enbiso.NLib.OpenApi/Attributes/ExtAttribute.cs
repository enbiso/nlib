using System;
using Microsoft.OpenApi.Any;

namespace Enbiso.NLib.OpenApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Field |
                    AttributeTargets.Enum)]
    public class ExtAttribute : Attribute
    {
        public readonly string Name;
        public readonly IOpenApiAny Value;

        public ExtAttribute(string name, IOpenApiAny value)
        {
            Name = name.StartsWith("x-") ? name : "x-" + name;
            Value = value;
        }
    }
}