using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus;

namespace StorefrontCommunity.Gallery.Tests.Fakes
{
    public sealed class FakeMessageBroker : IMessageBroker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EventBinding _eventBinding;

        public FakeMessageBroker(IServiceProvider serviceProvider, EventBinding eventBinding)
        {
            _serviceProvider = serviceProvider;
            _eventBinding = eventBinding;

            PublishedEvents = new List<Event<object>>();
        }

        public ICollection<Event<object>> PublishedEvents { get; }

        public void Publish<TPayload>(Event<TPayload> @event)
        {
            PublishedEvents.Add(new Event<object>
            {
                Id = @event.Id,
                CreatedAt = @event.CreatedAt,
                Name = @event.Name,
                Payload = @event.Payload
            });

            var type = _eventBinding.GetSubscriberType(@event.Name);

            if (type != null)
            {
                var subscriber = (IEventSubscriber)_serviceProvider.GetRequiredService(type);
                subscriber.Handle(JsonConvert.SerializeObject(@event));
            }
        }
    }
}
