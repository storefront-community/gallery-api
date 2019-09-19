using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storefront.Gallery.API.Authorization;
using Storefront.Gallery.API.Models.IntegrationModel.FileStorage;
using Storefront.Gallery.API.Models.ServiceModel;
using Storefront.Gallery.API.Models.TransferModel;

namespace Storefront.Gallery.API.Controllers
{
    /// <summary>
    /// Collection of pictures.
    /// </summary>
    [Route("{galleryName:gallery}/{imageName}"), Authorize]
    [ApiExplorerSettings(GroupName = "Pictures")]
    public sealed class PicturesController : Controller
    {
        private readonly IFileStorage _fileStorage;

        public PicturesController(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        /// <summary>
        /// Get picture in JPEG format.
        /// </summary>
        /// <param name="galleryName">Gallery name: **item** or **itemgroup**.</param>
        /// <param name="imageName">Image name. Represented by the item ID or group item ID.</param>
        /// <param name="imageSize">
        /// Image size: **default** (720x480), **cover** (1920x1280) or **thumbnail** (64x48).
        /// </param>
        /// <returns>Returns a picture.</returns>
        /// <response code="200">Image file.</response>
        /// <response code="404">Error: PICTURE_NOT_FOUND</response>
        [HttpGet, Route("{imageSize:regex(^(default|cover|thumbnail)$)}")]
        [Produces("image/jpeg")]
        [ProducesResponseType(statusCode: 200)]
        [ProducesResponseType(statusCode: 404, type: typeof(PictureNotFoundError))]
        public async Task<IActionResult> Get([FromRoute] string galleryName,
            [FromRoute] string imageName, [FromRoute] string imageSize)
        {
            var tenantId = User.Claims.TenantId();
            var gallery = new PictureGallery(_fileStorage);

            await gallery.Load(tenantId, galleryName, imageName, imageSize);

            if (gallery.PictureNotExists)
            {
                return new PictureNotFoundError();
            }

            return File(gallery.Picture.Stream, gallery.Picture.ContentType);
        }

        /// <summary>
        /// Add or update picture. Supports JPEG and PNG only.
        /// </summary>
        /// <param name="formData">Uploaded image using FormData.</param>
        /// <param name="galleryName">Gallery name: **item** or **itemgroup**.</param>
        /// <param name="imageName">Image name. Represented by the item ID or group item ID.</param>
        /// <param name="imageSize">
        /// Image size: **default** (720x480) or **cover** (1920x1280). The image will be converted to JPEG and resized.
        /// </param>
        /// <returns>No content.</returns>
        /// <response code="204">Picture has been added or updated.</response>
        [HttpPut, Route("{imageSize:regex(^(default|cover)$)}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(statusCode: 204)]
        public async Task<IActionResult> Save([FromForm] PictureFormData formData,
            [FromRoute] string galleryName, [FromRoute] string imageName, [FromRoute] string imageSize)
        {
            var tenantId = User.Claims.TenantId();
            var gallery = new PictureGallery(_fileStorage);

            using (var stream = new MemoryStream())
            {
                await formData.File.CopyToAsync(stream);
                await gallery.Save(tenantId, galleryName, imageName, imageSize, stream);
            }

            return NoContent();
        }
    }
}
