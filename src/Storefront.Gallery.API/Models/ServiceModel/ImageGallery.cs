using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Storefront.Gallery.API.Models.EventModel.Published;
using Storefront.Gallery.API.Models.IntegrationModel.EventBus;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;

namespace Storefront.Gallery.API.Models.ServiceModel
{
    public sealed class ImageGallery
    {
        public const string Standard = "standard";
        public const string Thumbnail = "thumbnail";
        public const string Cover = "cover";

        private readonly IFileStorage _fileStorage;
        private readonly IEventBus _eventBus;

        public ImageGallery(IFileStorage fileStorage, IEventBus eventBus)
        {
            _fileStorage = fileStorage;
            _eventBus = eventBus;
        }

        public StoredFile Image { get; private set; }
        public bool ImageNotExists { get; private set; }

        public async Task Load(long tenantId, string galleryName, string imageName, string imageSize)
        {
            var imageFullQualifiedName = ImageName(tenantId, galleryName, imageName, imageSize);

            Image = await _fileStorage.Read(imageFullQualifiedName);

            ImageNotExists = Image == null;
        }

        public async Task Save(long tenantId, string galleryName, string imageName, string imageSize, Stream stream)
        {
            switch (imageSize)
            {
                case Standard:
                {
                    await SaveDefault(tenantId, galleryName, imageName, stream);
                    await SaveThumbnail(tenantId, galleryName, imageName, stream);
                    break;
                }
                case Cover:
                {
                    await SaveCover(tenantId, galleryName, imageName, stream);
                    break;
                }
            }
        }

        private async Task SaveDefault(long tenantId, string galleryName, string imageName, Stream stream)
        {
            var storedFile = new StoredFile
            {
                Name = ImageName(tenantId, galleryName, imageName, Standard),
                ContentType = MediaTypeNames.Image.Jpeg,
                Stream = new ImageCompress(stream).Optimize(width: 720, height: 480, quality: 90)
            };

            await _fileStorage.Save(storedFile);

            _eventBus.Publish(new ImageCreatedEvent(storedFile));
        }

        private async Task SaveThumbnail(long tenantId, string galleryName, string imageName, Stream stream)
        {
            var storedFile = new StoredFile
            {
                Name = ImageName(tenantId, galleryName, imageName, Thumbnail),
                ContentType = MediaTypeNames.Image.Jpeg,
                Stream = new ImageCompress(stream).Optimize(width: 72, height: 48, quality: 20)
            };

            await _fileStorage.Save(storedFile);

            _eventBus.Publish(new ImageCreatedEvent(storedFile));
        }

        private async Task SaveCover(long tenantId, string galleryName, string imageName, Stream stream)
        {
            var storedFile = new StoredFile
            {
                Name = ImageName(tenantId, galleryName, imageName, Cover),
                ContentType = MediaTypeNames.Image.Jpeg,
                Stream = new ImageCompress(stream).Optimize(width: 1920, height: 1280, quality: 90)
            };

            await _fileStorage.Save(storedFile);

            _eventBus.Publish(new ImageCreatedEvent(storedFile));
        }

        private string ImageName(long tenantId, string galleryName, string imageName, string imageSize)
        {
            return $"{tenantId}-{imageName}.{galleryName}.{imageSize}.jpg";
        }
    }
}
