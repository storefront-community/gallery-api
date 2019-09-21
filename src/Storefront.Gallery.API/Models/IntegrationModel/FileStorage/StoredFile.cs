using System.IO;

namespace Storefront.Gallery.API.Models.IntegrationModel.FileStorage
{
    public sealed class StoredFile
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
    }
}
