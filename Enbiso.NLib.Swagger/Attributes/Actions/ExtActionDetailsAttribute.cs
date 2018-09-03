using System;

namespace Enbiso.NLib.Swagger.Attributes.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtActionDetailsAttribute : ExtActionAttribute
    {
        public ExtActionDetailsAttribute() : base("details", "Details", "{key}")
        {
        }

        public ExtActionDetailsAttribute(string display) : base("details", display, "{key}")
        {
        }

        public ExtActionDetailsAttribute(string display, string link) : base("details", display, link)
        {
        }
    }
}