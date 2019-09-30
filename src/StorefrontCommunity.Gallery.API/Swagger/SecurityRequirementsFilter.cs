using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StorefrontCommunity.Gallery.API.Swagger
{
    [ExcludeFromCodeCoverage]
    public sealed class SecurityRequirementsFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument document, DocumentFilterContext context)
        {
            document.Security = new List<IDictionary<string, IEnumerable<string>>>()
            {
                new Dictionary<string, IEnumerable<string>>()
                {
                    { "Bearer", new string[] { } },
                    { "Basic", new string[] { } },
                }
            };
        }
    }
}
