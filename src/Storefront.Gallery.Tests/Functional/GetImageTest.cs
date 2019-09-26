using System.Net;
using System.Threading.Tasks;
using Storefront.Gallery.API.Extensions;
using Storefront.Gallery.API.Models.TransferModel;
using Storefront.Gallery.Tests.Fakes;
using Xunit;

namespace Storefront.Gallery.Tests.Functional
{
    public sealed class GetImageTest
    {
        private const string ImageName = "1406";

        private readonly FakeApiServer _server;
        private readonly FakeApiToken _token;
        private readonly FakeApiClient _client;

        public GetImageTest()
        {
            _server = new FakeApiServer();
            _token = new FakeApiToken(_server.JwtOptions) { TenantId = 86 };
            _client = new FakeApiClient(_server, _token);
        }

        [Theory]
        [InlineData("item", "standard")]
        [InlineData("item", "thumbnail")]
        [InlineData("item", "cover")]
        [InlineData("itemgroup", "standard")]
        [InlineData("itemgroup", "thumbnail")]
        [InlineData("itemgroup", "cover")]
        public async Task ShouldRespond200(string gallery, string size)
        {
            var path = $"/{gallery}/{ImageName}/{size}";
            var response = await _client.GetAsync(path);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData("item", "standard")]
        [InlineData("item", "thumbnail")]
        [InlineData("item", "cover")]
        [InlineData("itemgroup", "standard")]
        [InlineData("itemgroup", "thumbnail")]
        [InlineData("itemgroup", "cover")]
        public async Task ShouldAlwaysRespondInJpegFormat(string gallery, string size)
        {
            var path = $"/{gallery}/{ImageName}/{size}";
            var response = await _client.GetAsync(path);

            Assert.Equal("image/jpeg", response.Content.Headers.ContentType.MediaType);
        }

        [Theory]
        [InlineData("item", "cover", "395d65a0f0aaf65f38cb1e4ce41ee3bc")]
        [InlineData("item", "standard", "86bed7f3716235f67dd33455470ce28c")]
        [InlineData("item", "thumbnail", "53841c18e965b9c2986b6533028fb616")]
        [InlineData("itemgroup", "cover", "ca135b7b8032a7ab8e3576affe2388dd")]
        [InlineData("itemgroup", "standard", "56b80871c2b8d2aa27e756af43732094")]
        [InlineData("itemgroup", "thumbnail", "e0b92beb7c005e1fce6bebcf9bf15957")]
        public async Task ShouldGetRequestedSize(string gallery, string size, string checksum)
        {
            var path = $"/{gallery}/{ImageName}/{size}";
            var response = await _client.GetAsync(path);
            var image = await response.Content.ReadAsStreamAsync();

            Assert.Equal(checksum, image.Checksum());
        }

        [Theory]
        [InlineData("item", "standard")]
        [InlineData("item", "thumbnail")]
        [InlineData("item", "cover")]
        [InlineData("itemgroup", "standard")]
        [InlineData("itemgroup", "thumbnail")]
        [InlineData("itemgroup", "cover")]
        public async Task ShouldRespond404IfImageDoesNotExist(string gallery, string size)
        {
            var path = $"/{gallery}/909/{size}";
            var response = await _client.GetAsync(path);
            var jsonResponse = await _client.ReadAsJsonAsync<ImageNotFoundError>(response);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("IMAGE_NOT_FOUND", jsonResponse.Error);
        }

        [Theory]
        [InlineData("item")]
        [InlineData("itemgroup")]
        public async Task ShouldRespond404ForInvalidSizeName(string gallery)
        {
            var path = $"/{gallery}/{ImageName}/nonsize";
            var response = await _client.GetAsync(path);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
