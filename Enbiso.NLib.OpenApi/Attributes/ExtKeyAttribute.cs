using System;
using Microsoft.OpenApi.Any;

namespace Enbiso.NLib.OpenApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtKeyAttribute : ExtAttribute
    {
        public ExtKeyAttribute() : base("x-key", new OpenApiBoolean(true))
        {
        }
    }
}