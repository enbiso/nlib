using System;
using Microsoft.OpenApi.Any;

namespace Enbiso.NLib.OpenApi.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtHiddenAttribute : ExtAttribute
    {
        public ExtHiddenAttribute() : base("x-hidden", new OpenApiBoolean(true))
        {
        }
    }
}