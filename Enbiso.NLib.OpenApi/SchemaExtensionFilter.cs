using System.Collections.Generic;
using System.Linq;
using Enbiso.NLib.OpenApi.Attributes;
using Enbiso.NLib.OpenApi.Attributes.Actions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Enbiso.NLib.OpenApi
{
    public class SchemaExtensionFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.ApiModel.Type;
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
        internal static void Add(this IDictionary<string, IOpenApiExtension> dictionary, ExtAttribute attribute)
        {
            var value = attribute.Value;
            if (attribute is ExtActionAttribute)
            {
                if (dictionary.ContainsKey(attribute.Name))
                {
                    (dictionary[attribute.Name] as OpenApiArray)?.Add(attribute.Value);
                    return;
                }
                value = new OpenApiArray { attribute.Value };
            }
            dictionary.Add(attribute.Name, value);
        }
    }
}