using System;

namespace Enbiso.NLib.OpenApi.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputWeekAttribute : ExtInputAttribute
    {
        public ExtInputWeekAttribute() : base("week")
        {
        }
    }
}