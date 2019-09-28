using System;
using System.Collections.Generic;

namespace Storefront.Gallery.API.Models.IntegrationModel.EventBus
{
    public class EventBinding
    {
        public EventBinding()
        {
            RoutingKey = string.Empty;
            Subscribers = new Dictionary<string, Type>();
        }

        public string RoutingKey { get; set; }
        public IDictionary<string, Type> Subscribers { get; }

        public void Route<TSubscriber>(string routingKey) where TSubscriber : IEventSubscriber
        {
            Subscribers.Add(routingKey, typeof(TSubscriber));
        }

        public Type GetSubscriberType(string routingKey)
        {
            if (!Subscribers.ContainsKey(routingKey))
            {
                return null;
            }

            return Subscribers[routingKey];
        }
    }
}
