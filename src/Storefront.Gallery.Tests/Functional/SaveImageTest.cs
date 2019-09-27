using System.IO;
using System.Net;
using System.Threading.Tasks;
using Storefront.Gallery.API.Models.EventModel.Published;
using Storefront.Gallery.API.Models.TransferModel;
using Storefront.Gallery.Tests.Factories;
using Storefront.Gallery.Tests.Fakes;
using Xunit;

namespace Storefront.Gallery.Tests.Functional
{
    public sealed class SaveImageTest
    {
        private const long Size5MB = 5242880;
        private const long Size10MB = 10485760;

        private readonly FakeApiServer _server;
        private readonly FakeApiToken _token;
        private readonly FakeApiClient _client;

        public SaveImageTest()
        {
            _server = new FakeApiServer();
            _token = new FakeApiToken(_server.JwtOptions);
            _client = new FakeApiClient(_server, _token);
        }

        [Theory]
        [InlineData("item", "standard")]
        [InlineData("item", "cover")]
        [InlineData("item-group", "standard")]
        [InlineData("item-group", "cover")]
        public async Task ShouldRespond204AfterSaveSuccessfully(string gallery, string display)
        {
            var imageId = ConstantFactory.Id;
            var path = $"/{gallery}/{imageId}/{display}";
            var image = await File.ReadAllBytesAsync($"{_server.Environment.ContentRootPath}/Fixtures/upload.jpg");
            var formData = new ImageFormData().Upload(image, "image/jpeg");
            var response = await _client.PutAsync(path, formData);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Theory]
        [InlineData("item", "image/jpeg")]
        [InlineData("item", "image/png")]
        [InlineData("item-group", "image/jpeg")]
        [InlineData("item-group", "image/png")]
        public async Task ShouldSaveStandardAndGenerateThumbnail(string gallery, string contentType)
        {
            var imageId = ConstantFactory.Id;
            var path = $"/{gallery}/{imageId}/standard";
            var image = await File.ReadAllBytesAsync($"{_server.Environment.ContentRootPath}/Fixtures/upload.jpg");
            var formData = new ImageFormData().Upload(image, contentType);
            var response = await _client.PutAsync(path, formData);
            var filenameStandard = $"{_token.TenantId}-{imageId}.{gallery}.standard.jpg";
            var fileNameThumbnail = $"{_token.TenantId}-{imageId}.{gallery}.thumbnail.jpg";

            Assert.Contains(_server.FileStorage.SavedFiles, file => file.Name == filenameStandard);
            Assert.Contains(_server.FileStorage.SavedFiles, file => file.Name == fileNameThumbnail);
        }

        [Theory]
        [InlineData("item", "image/jpeg")]
        [InlineData("item", "image/png")]
        [InlineData("item-group", "image/jpeg")]
        [InlineData("item-group", "image/png")]
        public async Task ShouldSaveCover(string gallery, string contentType)
        {
            var imageId = ConstantFactory.Id;
            var path = $"/{gallery}/{imageId}/cover";
            var image = await File.ReadAllBytesAsync($"{_server.Environment.ContentRootPath}/Fixtures/upload.jpg");
            var formData = new ImageFormData().Upload(image, contentType);
            var response = await _client.PutAsync(path, formData);
            var filenameCover = $"{_token.TenantId}-{imageId}.{gallery}.cover.jpg";

            Assert.Contains(_server.FileStorage.SavedFiles, file => file.Name == filenameCover);
        }

        [Theory]
        [InlineData("item", "standard")]
        [InlineData("item", "cover")]
        [InlineData("item-group", "standard")]
        [InlineData("item-group", "cover")]
        public async Task ShouldAlwaysSaveJpeg(string gallery, string display)
        {
            var imageId = ConstantFactory.Id;
            var path = $"/{gallery}/{imageId}/{display}";
            var fixture = $"{_server.Environment.ContentRootPath}/Fixtures/upload.jpg";
            var image = await File.ReadAllBytesAsync(fixture);
            var formData = new ImageFormData().Upload(image, "image/jpeg");
            var response = await _client.PutAsync(path, formData);

            Assert.All(_server.FileStorage.SavedFiles, file =>
                Assert.Equal("image/jpeg", file.ContentType));
        }

        [Theory]
        [InlineData("item")]
        [InlineData("item-group")]
        public async Task ShouldPublishEventAfterSavingCover(string gallery)
        {
            var imageId = ConstantFactory.Id;
            var path = $"/{gallery}/{imageId}/cover";
            var fixture = $"{_server.Environment.ContentRootPath}/Fixtures/upload.jpg";
            var image = await File.ReadAllBytesAsync(fixture);
            var formData = new ImageFormData().Upload(image, "image/jpeg");
            var response = await _client.PutAsync(path, formData);
            var filenameCover = $"{_token.TenantId}-{imageId}.{gallery}.cover.jpg";

            Assert.Contains(_server.EventBus.PublishedEvents, @event =>
                @event.Name == "gallery.image.created" &&
                (@event.Payload as ImageUploadedPayload).Filename == filenameCover &&
                (@event.Payload as ImageUploadedPayload).Size == 408688);
        }

        [Theory]
        [InlineData("item")]
        [InlineData("item-group")]
        public async Task ShouldPublishEventAfterSavingStandardAndThumbnail(string gallery)
        {
            var imageId = ConstantFactory.Id;
            var path = $"/{gallery}/{imageId}/standard";
            var fixture = $"{_server.Environment.ContentRootPath}/Fixtures/upload.jpg";
            var image = await File.ReadAllBytesAsync(fixture);
            var formData = new ImageFormData().Upload(image, "image/jpeg");
            var response = await _client.PutAsync(path, formData);
            var filenameStandard = $"{_token.TenantId}-{imageId}.{gallery}.standard.jpg";
            var fileNameThumbnail = $"{_token.TenantId}-{imageId}.{gallery}.thumbnail.jpg";

            Assert.Contains(_server.EventBus.PublishedEvents, @event =>
                @event.Name == "gallery.image.created" &&
                (@event.Payload as ImageUploadedPayload).Filename == filenameStandard &&
                (@event.Payload as ImageUploadedPayload).Size == 87714);

            Assert.Contains(_server.EventBus.PublishedEvents, @event =>
                @event.Name == "gallery.image.created" &&
                (@event.Payload as ImageUploadedPayload).Filename == fileNameThumbnail &&
                (@event.Payload as ImageUploadedPayload).Size == 1222);
        }

        [Theory]
        [InlineData("item")]
        [InlineData("item-group")]
        public async Task ShouldRespond404WhenTrySaveThumbnailDirectly(string gallery)
        {
            var imageId = ConstantFactory.Id;
            var path = $"/{gallery}/{imageId}/thumbnail";
            var image = new byte[Size5MB];
            var formData = new ImageFormData().Upload(image, "image/jpeg");
            var response = await _client.PutAsync(path, formData);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("item", "standard")]
        [InlineData("item", "cover")]
        [InlineData("item-group", "standard")]
        [InlineData("item-group", "cover")]
        public async Task ShouldRespond400ForUnacceptableContentType(string gallery, string display)
        {
            var imageId = ConstantFactory.Id;
            var path = $"/{gallery}/{imageId}/{display}";
            var image = new byte[Size5MB];
            var formData = new ImageFormData().Upload(image, "image/svg+xml");
            var response = await _client.PutAsync(path, formData);
            var jsonResponse = await _client.ReadAsJsonAsync<BadRequestError>(response);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(jsonResponse.Errors, error => error == "File must be an image in JPEG or PNG format.");
        }

        [Theory]
        [InlineData("item", "standard")]
        [InlineData("item", "cover")]
        [InlineData("item-group", "standard")]
        [InlineData("item-group", "cover")]
        public async Task ShouldRespond400ForSizeExceeded(string gallery, string display)
        {
            var imageId = ConstantFactory.Id;
            var path = $"/{gallery}/{imageId}/{display}";
            var image = new byte[Size10MB];
            var formData = new ImageFormData().Upload(image, "image/jpeg");
            var response = await _client.PutAsync(path, formData);
            var jsonResponse = await _client.ReadAsJsonAsync<BadRequestError>(response);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains(jsonResponse.Errors, error => error == "File must be an image with a maximum size of 5MB.");
        }

        [Theory]
        [InlineData("nongallery", "standard")]
        [InlineData("nongallery", "cover")]
        public async Task ShouldRespond404ForInvalidGallery(string gallery, string display)
        {
            var imageId = ConstantFactory.Id;
            var path = $"/{gallery}/{imageId}/{display}";
            var image = new byte[Size5MB];
            var formData = new ImageFormData().Upload(image, "image/jpeg");
            var response = await _client.PutAsync(path, formData);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
