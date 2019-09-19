using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Storefront.Gallery.API.Constraints
{
    public sealed class GalleryRouteConstraint : IRouteConstraint
    {
        public const string ConstraintName = "gallery";

        private const string Pattern = "^(item|itemgroup)$";

        public bool Match(HttpContext httpContext, IRouter route, string routeKey,
            RouteValueDictionary values, RouteDirection routeDirection)
        {
            object routeValue;

            if (values.TryGetValue(routeKey, out routeValue))
            {
                var routeValueString = Convert.ToString(routeValue, CultureInfo.InvariantCulture);

                return new Regex(Pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                    .IsMatch(routeValueString);
            }

            return false;
        }
    }
}
