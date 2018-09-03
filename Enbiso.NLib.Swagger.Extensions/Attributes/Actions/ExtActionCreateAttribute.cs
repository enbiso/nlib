using System;

namespace Enbiso.NLib.Swagger.Extensions.Attributes.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtActionCreateAttribute : ExtActionAttribute
    {
        public ExtActionCreateAttribute() : base("create", "Create", "create")
        {
        }

        public ExtActionCreateAttribute(string display) : base("create", display, "create")
        {
        }

        public ExtActionCreateAttribute(string display, string link) : base("create", display, link)
        {
        }
    }
}