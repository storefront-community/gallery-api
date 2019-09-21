using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Storefront.Gallery.API.Swagger
{
    [ExcludeFromCodeCoverage]
    public sealed class SwaggerExcludeFilter : IOperationFilter, ISchemaFilter
    {
        // Remove from request parameters
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var excludedProperties = context.ApiDescription.ActionDescriptor.Parameters
                .SelectMany(parameter => parameter.ParameterType.GetProperties())
                .Where(type => type.GetCustomAttribute<SwaggerExcludeAttribute>() != null);

            foreach (var excludedProperty in excludedProperties)
            {
                var excludedParameter = operation.Parameters
                    .Single(parameter => parameter.Name == excludedProperty.Name);

                operation.Parameters.Remove(excludedParameter);
            }
        }

        // Remove from response schema
        public void Apply(Schema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
            {
                return;
            }

            var excludedProperties = context.SystemType.GetProperties()
                .Where(type => type.GetCustomAttribute<SwaggerExcludeAttribute>() != null);

            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.ContainsKey(excludedProperty.Name))
                {
                    schema.Properties.Remove(excludedProperty.Name);
                }
            }
        }
    }
}
