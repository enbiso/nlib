using System;

namespace Enbiso.NLib.Swagger.Extensions.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputDateAttribute : ExtInputAttribute
    {
        public ExtInputDateAttribute() : base("date")
        {
        }
    }
}