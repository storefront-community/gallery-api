using System;
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

namespace StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus.RabbitMQ
{
    [ExcludeFromCodeCoverage]
    public sealed class RabbitMQBroker : IMessageBroker, IHostedService
    {
        private readonly RabbitMQOptions _options;
        private readonly IServiceProvider _serviceProvider;
        private readonly EventBinding _eventBinding;
        private readonly IConnection _connection;

        private IModel _channel;

        public RabbitMQBroker(IOptions<RabbitMQOptions> options, IConnection connection,
            IServiceProvider serviceProvider, EventBinding eventBinding)
        {
            _options = options.Value;
            _connection = connection;
            _serviceProvider = serviceProvider;
            _eventBinding = eventBinding;
        }

        public void Publish<TPayload>(Event<TPayload> @event)
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
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

            _channel.QueueBind(_options.Queue, _options.Exchange, _eventBinding.RoutingKey, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += HandleMessage;

            _channel.BasicConsume(_options.Queue, autoAck: false, consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Close();
            _connection.Close();

            return Task.CompletedTask;
        }

        private async Task HandleMessage(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var type = _eventBinding.GetSubscriberType(args.RoutingKey);

                    if (type != null)
                    {
                        var subscriber = ((IEventSubscriber)scope.ServiceProvider.GetRequiredService(type));
                        var message = Encoding.UTF8.GetString(args.Body);

                        await subscriber.Handle(message);
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
