using StorefrontCommunity.Gallery.API.Models.IntegrationModel.FileStorage;

namespace StorefrontCommunity.Gallery.API.Models.EventModel.Published
{
    public sealed class ImageCreatedPayload
    {
        public ImageCreatedPayload(StoredFile storedFile)
        {
            Filename = storedFile.Name;
            ContentType = storedFile.ContentType;
            Size = storedFile.Stream.Length;
        }

        public string Filename { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
    }
}
