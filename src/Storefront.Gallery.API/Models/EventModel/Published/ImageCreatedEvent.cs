using Storefront.Gallery.API.Models.IntegrationModel.EventBus;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;

namespace Storefront.Gallery.API.Models.EventModel.Published
{
    public sealed class ImageCreatedEvent : Event<ImageCreatedPayload>
    {
        public ImageCreatedEvent(StoredFile storedFile)
        {
            Name = "gallery.image.created";
            Payload = new ImageCreatedPayload(storedFile);
        }
    }
}
