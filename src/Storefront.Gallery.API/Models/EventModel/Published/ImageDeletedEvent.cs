using Storefront.Gallery.API.Models.IntegrationModel.EventBus;

namespace Storefront.Gallery.API.Models.EventModel.Published
{
    public class ImageDeletedEvent : Event<ImageDeletedPayload>
    {
        public ImageDeletedEvent(string filename)
        {
            Name = "gallery.image.deleted";
            Payload = new ImageDeletedPayload(filename);
        }
    }
}
