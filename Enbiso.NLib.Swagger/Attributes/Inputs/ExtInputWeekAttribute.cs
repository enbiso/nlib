using System;

namespace Enbiso.NLib.Swagger.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputWeekAttribute : ExtInputAttribute
    {
        public ExtInputWeekAttribute() : base("week")
        {
        }
    }
}