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

        private const string Pattern = "^(item|item-group)$";

        public bool Match(HttpContext httpContext, IRouter route, string routeKey,
            RouteValueDictionary values, RouteDirection routeDirection)
        {
            var routeValue = values[routeKey];
            var routeValueString = Convert.ToString(routeValue, CultureInfo.InvariantCulture);

            return new Regex(Pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)
                .IsMatch(routeValueString);
        }
    }
}
