namespace Storefront.Gallery.API.Models.EventModel.Published
{
    public sealed class ImageDeletedPayload
    {
        public ImageDeletedPayload(string filename)
        {
            Filename = filename;
        }

        public string Filename { get; set; }
    }
}
