using System;

namespace Enbiso.NLib.Swagger.Attributes.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtActionUpdateAttribute : ExtActionAttribute
    {
        public ExtActionUpdateAttribute() : base("update", "Update", "{key}/update")
        {
        }

        public ExtActionUpdateAttribute(string display) : base("update", display, "{key}/update")
        {
        }

        public ExtActionUpdateAttribute(string display, string link) : base("update", display, link)
        {
        }
    }
}