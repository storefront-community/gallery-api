using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace StorefrontCommunity.Gallery.API.Swagger
{
    [ExcludeFromCodeCoverage]
    public static class SwaggerExtensions
    {
        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Gallery API",
                    Version = "v1"
                });

                options.DocumentFilter<SecurityRequirementsFilter>();

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT authorization header using the Bearer scheme. Example: Bearer <TOKEN>",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                options.IncludeXmlComments(xmlPath);
                options.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
                options.TagActionsBy(api => new[] { api.GroupName });
                options.OperationFilter<SwaggerExcludeFilter>();
                options.SchemaFilter<SwaggerExcludeFilter>();
            });
        }

        public static void UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(setup =>
            {
                setup.SwaggerEndpoint("/swagger/v1/swagger.json", "Menu API");
                setup.DefaultModelsExpandDepth(-1);
                setup.DocExpansion(DocExpansion.List);
                setup.DocumentTitle = "Storefront Community";
                setup.RoutePrefix = string.Empty;
            });
        }
    }
}
