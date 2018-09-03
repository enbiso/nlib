﻿using System;

namespace Enbiso.NLib.Swagger.Extensions.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputColorAttribute : ExtInputAttribute
    {
        public ExtInputColorAttribute() : base("color")
        {
        }
    }
}
