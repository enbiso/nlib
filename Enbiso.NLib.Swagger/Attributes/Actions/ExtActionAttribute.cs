using System;

namespace Enbiso.NLib.Swagger.Attributes.Actions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExtActionAttribute : ExtAttribute
    {
        public ExtActionAttribute(string type) : base("x-action",
            new Action {Link = type, Type = type, Display = type})
        {
        }

        public ExtActionAttribute(string type, string display) : base("x-action",
            new Action {Link = type, Type = type, Display = display})
        {
        }

        public ExtActionAttribute(string type, string display, string link) : base("x-action",
            new Action {Link = link, Type = type, Display = display})
        {
        }

        public class Action
        {
            public string Type { get; set; }
            public string Display { get; set; }
            public string Link { get; set; }
        }
    }
}