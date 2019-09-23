namespace Storefront.Gallery.API.Models.IntegrationModel.EventBus
{
    public interface IEventBus
    {
        string Binding { get; set; }
        void Publish<TPayload>(Event<TPayload> @event);
        void Subscribe<TEventHandler>(string binding) where TEventHandler : EventHandler;
    }
}
