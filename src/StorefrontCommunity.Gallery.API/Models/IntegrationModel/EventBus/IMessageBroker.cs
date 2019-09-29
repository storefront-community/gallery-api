namespace StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus
{
    public interface IMessageBroker
    {
        void Publish<TPayload>(Event<TPayload> @event);
    }
}
