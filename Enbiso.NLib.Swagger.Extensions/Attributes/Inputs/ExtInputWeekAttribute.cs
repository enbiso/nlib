using System;

namespace Enbiso.NLib.Swagger.Extensions.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputWeekAttribute : ExtInputAttribute
    {
        public ExtInputWeekAttribute() : base("week")
        {
        }
    }
}