using System;

namespace Enbiso.NLib.Swagger.Extensions.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputDateTimeLocalAttribute : ExtInputAttribute
    {
        public ExtInputDateTimeLocalAttribute() : base("datetime-local")
        {
        }
    }
}