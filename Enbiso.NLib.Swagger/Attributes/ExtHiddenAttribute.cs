using System;

namespace Enbiso.NLib.Swagger.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtHiddenAttribute : ExtAttribute
    {
        public ExtHiddenAttribute() : base("x-hidden", true)
        {
        }
    }
}