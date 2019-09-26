using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Storefront.Gallery.API.Authorization;
using Storefront.Gallery.API.Constraints;
using Storefront.Gallery.API.Filters;
using Storefront.Gallery.API.Models.EventModel.Subscribed.Menu;
using Storefront.Gallery.API.Models.IntegrationModel.EventBus;
using Storefront.Gallery.API.Models.IntegrationModel.EventBus.RabbitMQ;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage.AmazonS3;
using Storefront.Gallery.API.Swagger;

namespace Storefront.Gallery.API
{
    [ExcludeFromCodeCoverage]
    public sealed class Startup
    {
        public IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RabbitMQOptions>(_configuration.GetSection("RabbitMQ"));

            services.AddMvc(options =>
            {
                options.Filters.Add(new RequestValidationFilter());
            });

            services.AddRouting(options =>
            {
                options.ConstraintMap.Add(GalleryRouteConstraint.ConstraintName, typeof(GalleryRouteConstraint));
            });

            services.AddDefaultCorsPolicy();
            services.AddJwtAuthentication(_configuration.GetSection("JWT"));
            services.AddSwaggerDocumentation();

            services.AddScoped<IFileStorage, AmazonS3Bucket>();

            services.AddScoped<ItemDeletedEvent>();
            services.AddScoped<ItemGroupDeletedEvent>();

            services.AddSingleton<IEventBus, RabbitMQBroker>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>();
                var broker = new RabbitMQBroker(options, serviceProvider);

                broker.RoutingKey = "menu.*.deleted";
                broker.Subscribe<ItemDeletedEvent>("menu.item.deleted");
                broker.Subscribe<ItemGroupDeletedEvent>("menu.item-group.deleted");

                return broker;
            });

            services.AddHostedService<RabbitMQBroker>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseSwaggerDocumentation();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
