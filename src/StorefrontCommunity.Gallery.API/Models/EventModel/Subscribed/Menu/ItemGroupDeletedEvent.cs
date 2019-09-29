using System.Threading.Tasks;
using Newtonsoft.Json;
using StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus;
using StorefrontCommunity.Gallery.API.Models.IntegrationModel.FileStorage;
using StorefrontCommunity.Gallery.API.Models.ServiceModel;

namespace StorefrontCommunity.Gallery.API.Models.EventModel.Subscribed.Menu
{
    public sealed class ItemGroupDeletedEvent : IEventSubscriber
    {
        private readonly IFileStorage _fileStorage;
        private readonly IMessageBroker _messageBroker;

        public ItemGroupDeletedEvent(IFileStorage fileStorage, IMessageBroker messageBroker)
        {
            _fileStorage = fileStorage;
            _messageBroker = messageBroker;
        }

        public async Task Handle(string message)
        {
            var json = JsonConvert.DeserializeObject<Event<ItemPayload>>(message);
            var imageGallery = new ImageGallery(_fileStorage, _messageBroker);

            await imageGallery.Delete(
                tenantId: json.Payload.TenantId,
                imageId: json.Payload.Id.ToString(),
                gallery: "item-group");
        }
    }
}
