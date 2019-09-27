using Storefront.Gallery.API.Models.IntegrationModel.EventBus;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;

namespace Storefront.Gallery.API.Models.EventModel.Published
{
    public sealed class ImageCreatedEvent : Event<ImageUploadedPayload>
    {
        public ImageCreatedEvent(StoredFile storedFile)
        {
            Name = "gallery.image.created";
            Payload = new ImageUploadedPayload(storedFile);
        }
    }
}
