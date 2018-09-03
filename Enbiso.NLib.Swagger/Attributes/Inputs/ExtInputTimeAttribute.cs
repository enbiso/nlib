using System;

namespace Enbiso.NLib.Swagger.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputTimeAttribute : ExtInputAttribute
    {
        public ExtInputTimeAttribute() : base("time")
        {
        }
    }
}