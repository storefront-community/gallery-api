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
    [Route("{gallery:gallery}/{image}"), Authorize]
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
        /// <param name="gallery">Gallery name: **item** or **item-group**.</param>
        /// <param name="image">Image ID (or name). Represented by the item ID or group item ID.</param>
        /// <param name="display">
        /// Image display size: **standard** (720x480), **cover** (1920x1280) or **thumbnail** (72x48).
        /// </param>
        /// <returns>Returns a image.</returns>
        /// <response code="200">Image file.</response>
        /// <response code="404">Error: IMAGE_NOT_FOUND</response>
        [HttpGet, Route("{display:regex(^(standard|cover|thumbnail)$)}")]
        [Produces("image/jpeg")]
        [ProducesResponseType(statusCode: 200)]
        [ProducesResponseType(statusCode: 404, type: typeof(ImageNotFoundError))]
        public async Task<IActionResult> Get([FromRoute] string gallery,
            [FromRoute] string image, [FromRoute] string display)
        {
            var tenantId = User.Claims.TenantId();
            var imageGallery = new ImageGallery(_fileStorage, _eventBus);

            await imageGallery.Load(tenantId, image, gallery, display);

            if (imageGallery.ImageNotExists)
            {
                return new ImageNotFoundError();
            }

            return File(imageGallery.Image.Stream, imageGallery.Image.ContentType);
        }

        /// <summary>
        /// Add or update image. Supports JPEG and PNG only.
        /// </summary>
        /// <param name="formData">Uploaded image using FormData.</param>
        /// <param name="gallery">Gallery name: **item** or **item-group**.</param>
        /// <param name="image">Image ID (or name). Represented by the item ID or group item ID.</param>
        /// <param name="display">
        /// Image display size: **standard** (720x480) or **cover** (1920x1280).
        /// The image will be resized and converted to JPEG at 72 DPI.
        /// </param>
        /// <returns>No content.</returns>
        /// <response code="204">Image has been added or updated.</response>
        [HttpPut, Route("{display:regex(^(standard|cover)$)}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(statusCode: 204)]
        public async Task<IActionResult> Save([FromForm] ImageFormData formData,
            [FromRoute] string gallery, [FromRoute] string image, [FromRoute] string display)
        {
            var tenantId = User.Claims.TenantId();
            var imageGallery = new ImageGallery(_fileStorage, _eventBus);

            using (var stream = new MemoryStream())
            {
                await formData.File.CopyToAsync(stream);
                await imageGallery.Save(tenantId, image, gallery, display, stream);
            }

            return NoContent();
        }
    }
}
