using System.IO;
using System.Net;
using System.Threading.Tasks;
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
        [InlineData("itemgroup", "standard")]
        [InlineData("itemgroup", "cover")]
        public async Task ShouldRespond204(string gallery, string size)
        {
            var filename = ConstantFactory.Id;
            var path = $"/{gallery}/{filename}/{size}";
            var image = await File.ReadAllBytesAsync($"{_server.Environment.ContentRootPath}/Fixtures/upload.jpg");
            var formData = new ImageFormData().Upload(image, "image/jpeg");
            var response = await _client.PutAsync(path, formData);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Theory]
        [InlineData("item", "image/jpeg")]
        [InlineData("item", "image/png")]
        [InlineData("itemgroup", "image/jpeg")]
        [InlineData("itemgroup", "image/png")]
        public async Task ShouldSavestandardSizeAndGenerateThumbnail(string gallery, string contentType)
        {
            var filename = ConstantFactory.Id;
            var path = $"/{gallery}/{filename}/standard";
            var image = await File.ReadAllBytesAsync($"{_server.Environment.ContentRootPath}/Fixtures/upload.jpg");
            var formData = new ImageFormData().Upload(image, contentType);
            var response = await _client.PutAsync(path, formData);
            var standardSize = $"{_token.TenantId}-{filename}.{gallery}.standard.jpg";
            var thumbnailSize = $"{_token.TenantId}-{filename}.{gallery}.thumbnail.jpg";

            Assert.Contains(_server.FileStorage.SavedFiles, file => file.Name == standardSize);
            Assert.Contains(_server.FileStorage.SavedFiles, file => file.Name == thumbnailSize);
        }

        [Theory]
        [InlineData("item", "image/jpeg")]
        [InlineData("item", "image/png")]
        [InlineData("itemgroup", "image/jpeg")]
        [InlineData("itemgroup", "image/png")]
        public async Task ShouldSaveCoverSize(string gallery, string contentType)
        {
            var filename = ConstantFactory.Id;
            var path = $"/{gallery}/{filename}/cover";
            var image = await File.ReadAllBytesAsync($"{_server.Environment.ContentRootPath}/Fixtures/upload.jpg");
            var formData = new ImageFormData().Upload(image, contentType);
            var response = await _client.PutAsync(path, formData);
            var coverSize = $"{_token.TenantId}-{filename}.{gallery}.cover.jpg";

            Assert.Contains(_server.FileStorage.SavedFiles, file => file.Name == coverSize);
        }

        [Theory]
        [InlineData("item")]
        [InlineData("itemgroup")]
        public async Task ShouldRespond404WhenTrySaveThumbnailDirectly(string gallery)
        {
            var filename = ConstantFactory.Id;
            var path = $"/{gallery}/{filename}/thumbnail";
            var image = new byte[Size5MB];
            var formData = new ImageFormData().Upload(image, "image/jpeg");
            var response = await _client.PutAsync(path, formData);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData("item", "standard")]
        [InlineData("item", "cover")]
        [InlineData("itemgroup", "standard")]
        [InlineData("itemgroup", "cover")]
        public async Task ShouldRespond400ForUnacceptableContentType(string gallery, string size)
        {
            var filename = ConstantFactory.Id;
            var path = $"/{gallery}/{filename}/{size}";
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
        [InlineData("itemgroup", "standard")]
        [InlineData("itemgroup", "cover")]
        public async Task ShouldRespond400ForSizeExceeded(string gallery, string size)
        {
            var filename = ConstantFactory.Id;
            var path = $"/{gallery}/{filename}/{size}";
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
        public async Task ShouldRespond404ForInvalidGallery(string gallery, string size)
        {
            var filename = ConstantFactory.Id;
            var path = $"/{gallery}/{filename}/{size}";
            var image = new byte[Size5MB];
            var formData = new ImageFormData().Upload(image, "image/jpeg");
            var response = await _client.PutAsync(path, formData);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
