using System;

namespace Enbiso.NLib.OpenApi.Attributes.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtActionDeleteAttribute : ExtActionAttribute
    {
        public ExtActionDeleteAttribute() : base("delete", "Delete", "{key}/delete")
        {
        }

        public ExtActionDeleteAttribute(string display) : base("delete", display, "{key}/delete")
        {
        }

        public ExtActionDeleteAttribute(string display, string link) : base("delete", display, link)
        {
        }
    }
}