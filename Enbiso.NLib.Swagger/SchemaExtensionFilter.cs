using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Enbiso.NLib.Swagger.Attributes;
using Enbiso.NLib.Swagger.Attributes.Actions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Enbiso.NLib.Swagger
{
    public class SchemaExtensionFilter : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaFilterContext context)
        {
            var type = context.SystemType.GetTypeInfo();
            var attributes = type.GetCustomAttributes(false).OfType<ExtAttribute>();
            foreach (var attribute in attributes)
                schema.Extensions.Add(attribute);

            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var propAttributes = property.GetCustomAttributes(false).OfType<ExtAttribute>();
                foreach (var attribute in propAttributes)
                {
                    var key = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
                    schema.Properties[key]?.Extensions.Add(attribute);
                }
            }
        }
    }

    internal static class DictionaryExtensions
    {
        internal static void Add(this IDictionary<string, object> dictionary, ExtAttribute attribute)
        {
            var value = attribute.Value;
            if (attribute is ExtActionAttribute)
            {
                if (dictionary.ContainsKey(attribute.Name))
                {
                    (dictionary[attribute.Name] as List<object>)?.Add(attribute.Value);
                    return;
                }
                value = new List<object> {attribute.Value};
            }
            dictionary.Add(attribute.Name, value);
        }
    }
}