using System;

namespace Enbiso.NLib.Swagger.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputDateAttribute : ExtInputAttribute
    {
        public ExtInputDateAttribute() : base("date")
        {
        }
    }
}