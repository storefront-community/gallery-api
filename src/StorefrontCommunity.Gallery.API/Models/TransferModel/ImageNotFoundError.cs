using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StorefrontCommunity.Gallery.API.Models.TransferModel
{
    public sealed class ImageNotFoundError : IActionResult
    {
        public ImageNotFoundError() { }

        public string Error { get; set; } = "IMAGE_NOT_FOUND";

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var json = new JsonResult(this) { StatusCode = 404 };
            await json.ExecuteResultAsync(context);
        }
    }
}
