using Storefront.Gallery.API.Models.IntegrationModel.EventBus;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;

namespace Storefront.Gallery.API.Models.EventModel.Published
{
    public class ImageDeletedEvent : Event<ImagePayload>
    {
        public ImageDeletedEvent(StoredFile storedFile)
        {
            Name = "gallery.image.deleted";
            Payload = new ImagePayload(storedFile);
        }
    }
}
