using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using StorefrontCommunity.Gallery.API.Models.TransferModel;

namespace StorefrontCommunity.Gallery.API.Filters
{
    public sealed class RequestValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ModelState.IsValid)
            {
                var errors = filterContext.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .ToList();

                filterContext.Result = new BadRequestError(errors);
            }
        }
    }
}
