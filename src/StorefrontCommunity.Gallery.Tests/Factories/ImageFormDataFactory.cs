using System.Net.Http;
using System.Net.Http.Headers;
using StorefrontCommunity.Gallery.API.Models.TransferModel;

namespace StorefrontCommunity.Gallery.Tests.Factories
{
    public static class ImageFormDataFactory
    {
        public static HttpContent Upload(this ImageFormData image, byte[] bytes, string contentType)
        {
            var file = new ByteArrayContent(bytes);
            file.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var formData = new MultipartFormDataContent();
            formData.Add(file, nameof(ImageFormData.File), nameof(ImageFormData.File));

            return formData;
        }
    }
}
