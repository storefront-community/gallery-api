using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Storefront.Gallery.API.Models.TransferModel
{
    public sealed class PictureNotFoundError : IActionResult
    {
        public PictureNotFoundError() { }

        public string Error { get; set; } = "PICTURE_NOT_FOUND";

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var json = new JsonResult(this) { StatusCode = 404 };
            await json.ExecuteResultAsync(context);
        }
    }
}
