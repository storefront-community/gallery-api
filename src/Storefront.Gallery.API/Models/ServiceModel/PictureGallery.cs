using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;

namespace Storefront.Gallery.API.Models.ServiceModel
{
    public sealed class PictureGallery
    {
        private readonly IFileStorage _fileStorage;

        public PictureGallery(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public StoredFile Picture { get; private set; }
        public bool PictureNotExists { get; private set; }

        public async Task Load(long tenantId, string galleryName, string imageName, string imageSize)
        {
            var pictureName = PictureName(tenantId, galleryName, imageName, imageSize);

            Picture = await _fileStorage.Read(pictureName);

            PictureNotExists = Picture == null;
        }

        public async Task Save(long tenantId, string galleryName, string imageName, string imageSize, Stream stream)
        {
            switch (imageSize)
            {
                case "default": await SaveDefaultSize(tenantId, galleryName, imageName, stream); break;
                case "cover": await SaveCoverSize(tenantId, galleryName, imageName, stream); break;
            }
        }

        private async Task SaveDefaultSize(long tenantId, string galleryName, string imageName, Stream stream)
        {
            var saveDefault = _fileStorage.Save(new StoredFile
            {
                Name = PictureName(tenantId, galleryName, imageName, "default"),
                ContentType = MediaTypeNames.Image.Jpeg,
                Stream = new ImageCompress(stream).Optimize(720, 480, quality: 90)
            });

            var saveThumbnail = _fileStorage.Save(new StoredFile
            {
                Name = PictureName(tenantId, galleryName, imageName, "thumbnail"),
                ContentType = MediaTypeNames.Image.Jpeg,
                Stream = new ImageCompress(stream).Optimize(64, 48, quality: 20)
            });

            await Task.WhenAll(saveDefault, saveThumbnail);
        }

        private async Task SaveCoverSize(long tenantId, string galleryName, string imageName, Stream stream)
        {
            await _fileStorage.Save(new StoredFile
            {
                Name = PictureName(tenantId, galleryName, imageName, "cover"),
                ContentType = MediaTypeNames.Image.Jpeg,
                Stream = new ImageCompress(stream).Optimize(1920, 1280, quality: 90)
            });
        }

        private string PictureName(long tenantId, string galleryName, string imageName, string imageSize)
        {
            return $"{tenantId}-{galleryName}-{imageName}.{imageSize}.jpeg";
        }
    }
}
