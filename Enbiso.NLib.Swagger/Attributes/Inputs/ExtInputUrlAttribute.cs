using System;

namespace Enbiso.NLib.Swagger.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputUrlAttribute : ExtInputAttribute
    {
        public ExtInputUrlAttribute() : base("url")
        {
        }
    }
}