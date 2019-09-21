using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Storefront.Gallery.API.Swagger;

namespace Storefront.Gallery.API.Models.TransferModel
{
    public sealed class ImageFormData
    {
        public const long Max5MB = 5242880;

        /// <summary>
        /// Image binary: JPEG or PNG.
        /// </summary>
        /// <value></value>
        [Required]
        public IFormFile File { get; set; }

        [SwaggerExclude]
        [RegularExpression("^image/(jpeg|png)$", ErrorMessage = "File must be an image in JPEG or PNG format.")]
        public string ContentType => File?.ContentType;

        [SwaggerExclude]
        [Range(0, Max5MB, ErrorMessage = "File must be an image with a maximum size of 5MB.")]
        public long Size => File?.Length ?? 0;

        public async Task<byte[]> ToByteArray()
        {
            using (var stream = new MemoryStream())
            {
                await File.CopyToAsync(stream);

                return stream.ToArray();
            }
        }
    }
}
