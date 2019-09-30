using System.IO;

namespace StorefrontCommunity.Gallery.API.Models.IntegrationModel.FileStorage
{
    public sealed class StoredFile
    {
        public StoredFile(Stream stream, string contentType, string name)
        {
            Name = name;
            ContentType = contentType;
            Stream = stream;
            Size = stream.Length;
        }

        public string Name { get; }
        public string ContentType { get; }
        public Stream Stream { get; }
        public long Size { get; }
    }
}
