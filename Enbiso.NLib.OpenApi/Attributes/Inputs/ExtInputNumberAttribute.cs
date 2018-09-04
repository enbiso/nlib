using System;

namespace Enbiso.NLib.OpenApi.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputNumberAttribute : ExtInputAttribute
    {
        public ExtInputNumberAttribute() : base("number")
        {
        }
    }
}