using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefront.Gallery.API.Authorization;
using Storefront.Gallery.API.Models.IntegrationModel.EventBus;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;
using Storefront.Gallery.API.Models.ServiceModel;
using Storefront.Gallery.API.Models.TransferModel;

namespace Storefront.Gallery.API.Controllers
{
    /// <summary>
    /// Collection of images.
    /// </summary>
    [Route("{galleryName:gallery}/{imageName}"), Authorize]
    [ApiExplorerSettings(GroupName = "Images")]
    public sealed class ImagesController : Controller
    {
        private readonly IFileStorage _fileStorage;
        private readonly IEventBus _eventBus;

        public ImagesController(IFileStorage fileStorage, IEventBus eventBus)
        {
            _fileStorage = fileStorage;
            _eventBus = eventBus;
        }

        /// <summary>
        /// Get image in JPEG format.
        /// </summary>
        /// <param name="galleryName">Gallery name: **item** or **itemgroup**.</param>
        /// <param name="imageName">Image name. Represented by the item ID or group item ID.</param>
        /// <param name="imageSize">
        /// Image size: **default** (720x480), **cover** (1920x1280) or **thumbnail** (72x48).
        /// </param>
        /// <returns>Returns a image.</returns>
        /// <response code="200">Image file.</response>
        /// <response code="404">Error: IMAGE_NOT_FOUND</response>
        [HttpGet, Route("{imageSize:regex(^(default|cover|thumbnail)$)}")]
        [Produces("image/jpeg")]
        [ProducesResponseType(statusCode: 200)]
        [ProducesResponseType(statusCode: 404, type: typeof(ImageNotFoundError))]
        public async Task<IActionResult> Get([FromRoute] string galleryName,
            [FromRoute] string imageName, [FromRoute] string imageSize)
        {
            var tenantId = User.Claims.TenantId();
            var gallery = new ImageGallery(_fileStorage, _eventBus);

            await gallery.Load(tenantId, galleryName, imageName, imageSize);

            if (gallery.ImageNotExists)
            {
                return new ImageNotFoundError();
            }

            return File(gallery.Image.Stream, gallery.Image.ContentType);
        }

        /// <summary>
        /// Add or update image. Supports JPEG and PNG only.
        /// </summary>
        /// <param name="formData">Uploaded image using FormData.</param>
        /// <param name="galleryName">Gallery name: **item** or **itemgroup**.</param>
        /// <param name="imageName">Image name. Represented by the item ID or group item ID.</param>
        /// <param name="imageSize">
        /// Image size: **default** (720x480) or **cover** (1920x1280). The image will be converted to JPEG and resized.
        /// </param>
        /// <returns>No content.</returns>
        /// <response code="204">Image has been added or updated.</response>
        [HttpPut, Route("{imageSize:regex(^(default|cover)$)}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(statusCode: 204)]
        public async Task<IActionResult> Save([FromForm] ImageFormData formData,
            [FromRoute] string galleryName, [FromRoute] string imageName, [FromRoute] string imageSize)
        {
            var tenantId = User.Claims.TenantId();
            var gallery = new ImageGallery(_fileStorage, _eventBus);

            using (var stream = new MemoryStream())
            {
                await formData.File.CopyToAsync(stream);
                await gallery.Save(tenantId, galleryName, imageName, imageSize, stream);
            }

            return NoContent();
        }
    }
}
