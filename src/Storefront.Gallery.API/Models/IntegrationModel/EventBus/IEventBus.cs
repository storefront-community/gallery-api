namespace Storefront.Gallery.API.Models.IntegrationModel.EventBus
{
    public interface IEventBus
    {
        string RoutingKey { get; set; }
        void Publish(IEvent @event);
        void Subscribe<TEventHandler>(string name) where TEventHandler : IEventHandler;
    }
}
