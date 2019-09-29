using StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus;
using StorefrontCommunity.Gallery.API.Models.IntegrationModel.FileStorage;

namespace StorefrontCommunity.Gallery.API.Models.EventModel.Published
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
