using System;

namespace Enbiso.NLib.Swagger.Extensions.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputTelAttribute : ExtInputAttribute
    {
        public ExtInputTelAttribute() : base("tel")
        {
        }
    }
}