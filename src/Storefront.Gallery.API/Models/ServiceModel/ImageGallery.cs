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
        public const string StandardDisplay = "standard";
        public const string ThumbnailDisplay = "thumbnail";
        public const string CoverDisplay = "cover";

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
            Image = await _fileStorage.Read(FullQualifiedFilename(tenantId, gallery, filename, display));

            ImageNotExists = Image == null;
        }

        public async Task Save(long tenantId, string gallery, string filename, string display, Stream image)
        {
            switch (display)
            {
                case StandardDisplay:
                {
                    await Task.WhenAll(
                        SaveStandard(tenantId, gallery, filename, image),
                        SaveThumbnail(tenantId, gallery, filename, image)
                    );
                    break;
                }
                case CoverDisplay:
                {
                    await SaveCover(tenantId, gallery, filename, image);
                    break;
                }
            }
        }

        public async Task Delete(long tenantId, string gallery, string filename)
        {
            var filenameCover = FullQualifiedFilename(tenantId, gallery, filename, CoverDisplay);
            var filenameStandard = FullQualifiedFilename(tenantId, gallery, filename, StandardDisplay);
            var filenameThumbnail = FullQualifiedFilename(tenantId, gallery, filename, ThumbnailDisplay);

            await _fileStorage.Delete(filenameCover);
            _eventBus.Publish(new ImageDeletedEvent(filenameCover));

            await _fileStorage.Delete(filenameStandard);
            _eventBus.Publish(new ImageDeletedEvent(filenameStandard));

            await _fileStorage.Delete(filenameThumbnail);
            _eventBus.Publish(new ImageDeletedEvent(filenameThumbnail));
        }

        private async Task SaveStandard(long tenantId, string gallery, string filename, Stream image)
        {
            var storedFile = new StoredFile
            {
                ContentType = MediaTypeNames.Image.Jpeg,
                Name = FullQualifiedFilename(tenantId, gallery, filename, StandardDisplay),
                Stream = new ImageCompress(image).Optimize(width: 720, height: 480, quality: 90)
            };

            await _fileStorage.Save(storedFile);

            _eventBus.Publish(new ImageCreatedEvent(storedFile));
        }

        private async Task SaveThumbnail(long tenantId, string gallery, string filename, Stream image)
        {
            var storedFile = new StoredFile
            {
                ContentType = MediaTypeNames.Image.Jpeg,
                Name = FullQualifiedFilename(tenantId, gallery, filename, ThumbnailDisplay),
                Stream = new ImageCompress(image).Optimize(width: 72, height: 48, quality: 20)
            };

            await _fileStorage.Save(storedFile);

            _eventBus.Publish(new ImageCreatedEvent(storedFile));
        }

        private async Task SaveCover(long tenantId, string gallery, string filename, Stream image)
        {
            var storedFile = new StoredFile
            {
                ContentType = MediaTypeNames.Image.Jpeg,
                Name = FullQualifiedFilename(tenantId, gallery, filename, CoverDisplay),
                Stream = new ImageCompress(image).Optimize(width: 1920, height: 1280, quality: 90)
            };

            await _fileStorage.Save(storedFile);

            _eventBus.Publish(new ImageCreatedEvent(storedFile));
        }

        private string FullQualifiedFilename(long tenantId, string gallery, string filename, string display)
        {
            return $"{tenantId}-{filename}.{gallery}.{display}.jpg";
        }
    }
}
