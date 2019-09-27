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

        public async Task Load(long tenantId, string gallery, string filename, string display)
        {
            var fileFullQualifiedName = ImageName(tenantId, gallery, filename, display);

            Image = await _fileStorage.Read(fileFullQualifiedName);

            ImageNotExists = Image == null;
        }

        public async Task Save(long tenantId, string gallery, string filename, string display, Stream image)
        {
            switch (display)
            {
                case Standard:
                {
                    await SaveDefault(tenantId, gallery, filename, image);
                    await SaveThumbnail(tenantId, gallery, filename, image);
                    break;
                }
                case Cover:
                {
                    await SaveCover(tenantId, gallery, filename, image);
                    break;
                }
            }
        }

        private async Task SaveDefault(long tenantId, string gallery, string filename, Stream image)
        {
            var storedFile = new StoredFile
            {
                Name = ImageName(tenantId, gallery, filename, Standard),
                ContentType = MediaTypeNames.Image.Jpeg,
                Stream = new ImageCompress(image).Optimize(width: 720, height: 480, quality: 90)
            };

            await _fileStorage.Save(storedFile);

            _eventBus.Publish(new ImageCreatedEvent(storedFile));
        }

        private async Task SaveThumbnail(long tenantId, string gallery, string filename, Stream image)
        {
            var storedFile = new StoredFile
            {
                Name = ImageName(tenantId, gallery, filename, Thumbnail),
                ContentType = MediaTypeNames.Image.Jpeg,
                Stream = new ImageCompress(image).Optimize(width: 72, height: 48, quality: 20)
            };

            await _fileStorage.Save(storedFile);

            _eventBus.Publish(new ImageCreatedEvent(storedFile));
        }

        private async Task SaveCover(long tenantId, string gallery, string filename, Stream image)
        {
            var storedFile = new StoredFile
            {
                Name = ImageName(tenantId, gallery, filename, Cover),
                ContentType = MediaTypeNames.Image.Jpeg,
                Stream = new ImageCompress(image).Optimize(width: 1920, height: 1280, quality: 90)
            };

            await _fileStorage.Save(storedFile);

            _eventBus.Publish(new ImageCreatedEvent(storedFile));
        }

        private string ImageName(long tenantId, string gallery, string filename, string display)
        {
            return $"{tenantId}-{filename}.{gallery}.{display}.jpg";
        }
    }
}
