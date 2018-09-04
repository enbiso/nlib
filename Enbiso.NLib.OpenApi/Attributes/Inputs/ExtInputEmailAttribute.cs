using System;

namespace Enbiso.NLib.OpenApi.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputEmailAttribute : ExtInputAttribute
    {
        public ExtInputEmailAttribute() : base("email")
        {
        }
    }
}