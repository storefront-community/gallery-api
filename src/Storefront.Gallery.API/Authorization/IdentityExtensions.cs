using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Storefront.Gallery.API.Authorization
{
    public static class IdentityExtensions
    {
        public static long TenantId(this IEnumerable<Claim> claims)
        {
            var claim = claims.FirstOrDefault(c => c.Type == nameof(TenantId));
            var tenantId = 0L;

            Int64.TryParse(claim?.Value, out tenantId);

            return tenantId;
        }

        public static long UserId(this IEnumerable<Claim> claims)
        {
            var claim = claims.FirstOrDefault(c => c.Type == nameof(UserId));
            var userId = 0L;

            Int64.TryParse(claim?.Value, out userId);

            return userId;
        }
    }
}
