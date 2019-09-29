using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace StorefrontCommunity.Gallery.API.Models.TransferModel
{
    public sealed class BadRequestError : IActionResult
    {
        public BadRequestError() { }

        public BadRequestError(params string[] errors)
        {
            Errors = errors;
        }

        public BadRequestError(IEnumerable<ModelError> modelErrors)
        {
            Errors = modelErrors
                .Select(modelError => ErrorMessage(modelError))
                .ToList();
        }

        public IEnumerable<string> Errors { get; set; }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var json = new JsonResult(this) { StatusCode = 400 };
            await json.ExecuteResultAsync(context);
        }

        private string ErrorMessage(ModelError modelError)
        {
            if (!string.IsNullOrWhiteSpace(modelError.ErrorMessage))
            {
                return modelError.ErrorMessage;
            }

            if (modelError.Exception is JsonReaderException readerException)
            {
                return readerException.Message;
            }

            if (modelError.Exception is JsonSerializationException serializationException)
            {
                var matches = Regex.Matches(serializationException.Message, "Path.*");

                if (matches.Any())
                {
                    return $"Parse error: {matches.First().Value}";
                }
            }

            return "Could not read the request body.";
        }
    }
}
