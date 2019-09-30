using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using StorefrontCommunity.Gallery.API.Models.EventModel.Published;
using StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus;
using StorefrontCommunity.Gallery.API.Models.IntegrationModel.FileStorage;

namespace StorefrontCommunity.Gallery.API.Models.ServiceModel
{
    public sealed class ImageGallery
    {
        public const string StandardDisplay = "standard";
        public const string ThumbnailDisplay = "thumbnail";
        public const string CoverDisplay = "cover";

        private readonly IFileStorage _fileStorage;
        private readonly IMessageBroker _messageBroker;

        public ImageGallery(IFileStorage fileStorage, IMessageBroker messageBroker)
        {
            _fileStorage = fileStorage;
            _messageBroker = messageBroker;
        }

        public StoredFile Image { get; private set; }
        public bool ImageNotExists { get; private set; }

        public async Task Load(long tenantId, string imageId, string gallery,string display)
        {
            Image = await _fileStorage.Read(Filename(tenantId, imageId, gallery, display));

            ImageNotExists = Image == null;
        }

        public async Task Save(long tenantId, string imageId, string gallery, string display, Stream image)
        {
            switch (display)
            {
                case StandardDisplay:
                {
                    await Task.WhenAll(
                        SaveStandard(tenantId, imageId, gallery, image),
                        SaveThumbnail(tenantId, imageId, gallery, image)
                    );
                    break;
                }
                case CoverDisplay:
                {
                    await SaveCover(tenantId, imageId, gallery, image);
                    break;
                }
            }
        }

        public async Task Delete(long tenantId, string imageId, string gallery)
        {
            var filenames = new[]
            {
                Filename(tenantId, imageId, gallery, CoverDisplay),
                Filename(tenantId, imageId, gallery, StandardDisplay),
                Filename(tenantId, imageId, gallery, ThumbnailDisplay)
            };

            foreach (var filename in filenames)
            {
                await _fileStorage.Delete(filename);
                _messageBroker.Publish(new ImageDeletedEvent(filename));
            }
        }

        private async Task SaveStandard(long tenantId, string imageId, string gallery, Stream image)
        {
            var storedFile = new StoredFile(
                contentType: MediaTypeNames.Image.Jpeg,
                name: Filename(tenantId, imageId, gallery, StandardDisplay),
                stream: new ImageCompress(image).Optimize(width: 720, height: 480, quality: 90)
            );

            await _fileStorage.Save(storedFile);

            _messageBroker.Publish(new ImageCreatedEvent(storedFile));
        }

        private async Task SaveThumbnail(long tenantId, string imageId, string gallery, Stream image)
        {
            var storedFile = new StoredFile(
                contentType: MediaTypeNames.Image.Jpeg,
                name: Filename(tenantId, imageId, gallery, ThumbnailDisplay),
                stream: new ImageCompress(image).Optimize(width: 72, height: 48, quality: 20)
            );

            await _fileStorage.Save(storedFile);

            _messageBroker.Publish(new ImageCreatedEvent(storedFile));
        }

        private async Task SaveCover(long tenantId, string imageId, string gallery, Stream image)
        {
            var storedFile = new StoredFile(
                contentType: MediaTypeNames.Image.Jpeg,
                name: Filename(tenantId, imageId, gallery, CoverDisplay),
                stream: new ImageCompress(image).Optimize(width: 1920, height: 1280, quality: 90)
            );

            await _fileStorage.Save(storedFile);

            _messageBroker.Publish(new ImageCreatedEvent(storedFile));
        }

        private string Filename(long tenantId, string imageId, string gallery, string display)
        {
            return $"{tenantId}-{imageId}.{gallery}.{display}.jpg";
        }
    }
}
