using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;

namespace Storefront.Gallery.API.Models.EventModel.Published
{
    public sealed class ImagePayload
    {
        public ImagePayload(StoredFile storedFile)
        {
            Name = storedFile.Name;
            ContentType = storedFile.ContentType;
            Size = storedFile.Stream.Length;
        }

        public string Name { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
    }
}
