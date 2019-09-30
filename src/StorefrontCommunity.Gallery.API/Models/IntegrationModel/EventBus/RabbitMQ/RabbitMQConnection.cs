using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus.RabbitMQ
{
    [ExcludeFromCodeCoverage]
    public static class RabbitMQConnection
    {
        public static void AddRabbitMQConnection(this IServiceCollection services)
        {
            services.AddSingleton<IConnection>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>();

                var connectionFactory = new ConnectionFactory()
                {
                    Uri = new Uri(options.Value.Host),
                    DispatchConsumersAsync = true,
                    AutomaticRecoveryEnabled = true
                };

                return connectionFactory.CreateConnection();
            });
        }
    }
}
