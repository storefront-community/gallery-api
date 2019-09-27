using System.Threading.Tasks;
using Newtonsoft.Json;
using Storefront.Gallery.API.Models.IntegrationModel.EventBus;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;
using Storefront.Gallery.API.Models.ServiceModel;

namespace Storefront.Gallery.API.Models.EventModel.Subscribed.Menu
{
    public sealed class ItemDeletedEvent : IEventHandler
    {
        private readonly IFileStorage _fileStorage;
        private readonly IEventBus _eventBus;

        public ItemDeletedEvent(IFileStorage fileStorage, IEventBus eventBus)
        {
            _fileStorage = fileStorage;
            _eventBus = eventBus;
        }

        public async Task Handle(string message)
        {
            var json = JsonConvert.DeserializeObject<Event<ItemPayload>>(message);
            var imageGallery = new ImageGallery(_fileStorage, _eventBus);

            await imageGallery.Delete(
                tenantId: json.Payload.TenantId,
                filename: json.Payload.Id.ToString(),
                gallery: "item");
        }
    }
}
