using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sentry;

namespace Storefront.Gallery.API.Models.IntegrationModel.EventBus.RabbitMQ
{
    [ExcludeFromCodeCoverage]
    public sealed class RabbitMQBroker : IEventBus, IHostedService
    {
        private readonly RabbitMQOptions _options;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, Type> _handlers;

        private IConnection _connection;
        private IModel _channel;

        public RabbitMQBroker(IOptions<RabbitMQOptions> options, IServiceProvider serviceProvider)
        {
            _options = options.Value;
            _serviceProvider = serviceProvider;
            _handlers = new Dictionary<string, Type>();
        }

        public string RoutingKey { get; set; }

        public void Publish(IEvent @event)
        {
            using (var channel = _connection.CreateModel())
            {
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(
                    exchange: _options.Exchange,
                    routingKey: @event.Name,
                    mandatory: false,
                    basicProperties: null,
                    body: body);
            }
        }

        public void Subscribe<TEventHandler>(string name) where TEventHandler : IEventHandler
        {
            _handlers.Add(name, typeof(TEventHandler));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var connectionFactory = new ConnectionFactory()
            {
                Uri = new Uri(_options.Host),
                AutomaticRecoveryEnabled = true
            };

            _connection = connectionFactory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                exchange: _options.Exchange,
                type: "topic",
                durable: false,
                autoDelete: true,
                arguments: null);

            _channel.QueueDeclare(
                queue: _options.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.QueueBind(_options.Queue, _options.Exchange, RoutingKey, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += HandleMessage;

            _channel.BasicConsume(_options.Queue, autoAck: false, consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection?.Close();

            return Task.CompletedTask;
        }

        private async Task HandleMessage(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var handlerType = default(Type);

                    if (_handlers.TryGetValue(args.RoutingKey, out handlerType))
                    {
                        var handler = ((IEventHandler)scope.ServiceProvider.GetRequiredService(handlerType));
                        var message = Encoding.UTF8.GetString(args.Body);

                        await handler.Handle(message);
                    }
                    else
                    {
                        SentrySdk.CaptureMessage($"RabbitMQ: No subscribers to event '{args.RoutingKey}'.");
                    }
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
