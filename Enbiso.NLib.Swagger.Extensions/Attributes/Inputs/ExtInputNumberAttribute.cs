using System;

namespace Enbiso.NLib.Swagger.Extensions.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputNumberAttribute : ExtInputAttribute
    {
        public ExtInputNumberAttribute() : base("number")
        {
        }
    }
}