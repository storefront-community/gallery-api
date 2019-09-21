using System;
using System.Diagnostics.CodeAnalysis;

namespace Storefront.Gallery.API.Swagger
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SwaggerExcludeAttribute : Attribute { }
}
