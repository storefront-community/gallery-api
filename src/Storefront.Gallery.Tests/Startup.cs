using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Storefront.Gallery.API.Authorization;
using Storefront.Gallery.API.Constraints;
using Storefront.Gallery.API.Filters;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;
using Storefront.Gallery.Tests.Fakes;

namespace Storefront.Gallery.Tests
{
    public sealed class Startup
    {
        public readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(new RequestValidationFilter());
            })
            .AddApplicationPart(typeof(Storefront.Gallery.API.Startup).Assembly);

            services.AddRouting(options =>
            {
                options.ConstraintMap.Add(GalleryRouteConstraint.ConstraintName, typeof(GalleryRouteConstraint));
            });

            services.AddDefaultCorsPolicy();
            services.AddJwtAuthentication(_configuration.GetSection("JWT"));

            services.AddSingleton<IFileStorage, FakeFileStorage>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
