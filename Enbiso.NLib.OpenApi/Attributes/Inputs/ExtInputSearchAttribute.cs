using System;

namespace Enbiso.NLib.OpenApi.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputSearchAttribute : ExtInputAttribute
    {
        public ExtInputSearchAttribute() : base("search")
        {
        }
    }
}