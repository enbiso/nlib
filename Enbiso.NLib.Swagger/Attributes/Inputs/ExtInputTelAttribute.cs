using System;

namespace Enbiso.NLib.Swagger.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputTelAttribute : ExtInputAttribute
    {
        public ExtInputTelAttribute() : base("tel")
        {
        }
    }
}