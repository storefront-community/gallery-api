using StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus;

namespace StorefrontCommunity.Gallery.API.Models.EventModel.Published
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
