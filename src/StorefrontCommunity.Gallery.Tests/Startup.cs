using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StorefrontCommunity.Gallery.API.Authorization;
using StorefrontCommunity.Gallery.API.Constraints;
using StorefrontCommunity.Gallery.API.Filters;
using StorefrontCommunity.Gallery.API.Models.EventModel.Subscribed.Menu;
using StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus;
using StorefrontCommunity.Gallery.API.Models.IntegrationModel.FileStorage;
using StorefrontCommunity.Gallery.Tests.Fakes;

namespace StorefrontCommunity.Gallery.Tests
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
            .AddApplicationPart(typeof(StorefrontCommunity.Gallery.API.Startup).Assembly);

            services.AddRouting(options =>
            {
                options.ConstraintMap.Add(GalleryRouteConstraint.ConstraintName, typeof(GalleryRouteConstraint));
            });

            services.AddDefaultCorsPolicy();
            services.AddJwtAuthentication(_configuration.GetSection("JWT"));

            services.AddSingleton<IFileStorage, FakeFileStorage>();
            services.AddSingleton<IMessageBroker, FakeMessageBroker>();

            services.AddScoped<ItemDeletedEvent>();
            services.AddScoped<ItemGroupDeletedEvent>();

            services.AddSingleton<EventBinding>(serviceProvider =>
            {
                var binding = new EventBinding();

                binding.RoutingKey = "menu.*.deleted";
                binding.Route<ItemDeletedEvent>("menu.item.deleted");
                binding.Route<ItemGroupDeletedEvent>("menu.item-group.deleted");

                return binding;
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
