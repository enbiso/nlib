using System;

namespace Enbiso.NLib.Swagger.Extensions.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputMonthAttribute : ExtInputAttribute
    {
        public ExtInputMonthAttribute() : base("month")
        {
        }
    }
}