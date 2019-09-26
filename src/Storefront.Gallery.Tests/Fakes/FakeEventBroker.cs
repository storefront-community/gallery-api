using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Storefront.Gallery.API.Models.IntegrationModel.EventBus;

namespace Storefront.Gallery.Tests.Fakes
{
    public sealed class FakeEventBroker : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, Type> _handlers;

        public FakeEventBroker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _handlers = new Dictionary<string, Type>();

            PublishedEvents = new List<IEvent>();
        }

        public string RoutingKey { get; set; }
        public ICollection<IEvent> PublishedEvents { get; }

        public void Publish(IEvent @event)
        {
            PublishedEvents.Add(@event);

            var handlerType = default(Type);

            if (_handlers.TryGetValue(@event.Name, out handlerType))
            {
                var handler = (IEventHandler)_serviceProvider
                    .GetRequiredService(_handlers[@event.Name]);

                handler.Handle(JsonConvert.SerializeObject(@event));
            }
        }

        public void Subscribe<TEventHandler>(string name) where TEventHandler : IEventHandler
        {
            _handlers.Add(name, typeof(TEventHandler));
        }
    }
}
