﻿using System;

namespace Enbiso.NLib.OpenApi.Attributes.Inputs
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ExtInputAttribute : ExtAttribute
    {
        public ExtInputAttribute(string type) : base("x-input", type)
        {
        }
    }
}