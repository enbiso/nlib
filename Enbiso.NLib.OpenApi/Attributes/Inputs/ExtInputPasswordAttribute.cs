using System;

namespace Enbiso.NLib.OpenApi.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputPasswordAttribute : ExtInputAttribute
    {
        public ExtInputPasswordAttribute() : base("password")
        {
        }
    }
}