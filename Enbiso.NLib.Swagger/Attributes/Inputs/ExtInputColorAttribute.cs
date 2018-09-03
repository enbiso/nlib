using System;

namespace Enbiso.NLib.Swagger.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputColorAttribute : ExtInputAttribute
    {
        public ExtInputColorAttribute() : base("color")
        {
        }
    }
}
