using System;

namespace Enbiso.NLib.OpenApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Field |
                    AttributeTargets.Enum)]
    public class ExtAttribute : Attribute
    {
        public readonly string Name;
        public readonly object Value;

        public ExtAttribute(string name, object value)
        {
            Name = name.StartsWith("x-") ? name : "x-" + name;
            Value = value;
        }
    }
}