using System;

namespace Enbiso.NLib.Swagger.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputAttribute : ExtAttribute
    {
        public ExtInputAttribute(string type) : base("x-input", type)
        {
        }
    }
}