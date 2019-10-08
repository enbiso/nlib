using System;
using Microsoft.OpenApi.Any;

namespace Enbiso.NLib.OpenApi.Attributes.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtActionAttribute : ExtAttribute
    {
       
        public ExtActionAttribute(string type, string display, string link) : base("x-action",
            new OpenApiObject
            {
                ["type"] = new OpenApiString(type),
                ["display"] = new OpenApiString(display),
                ["link"] = new OpenApiString(link),
            })
        {
        }
    }
}