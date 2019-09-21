using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Storefront.Gallery.API.Authorization;

namespace Storefront.Gallery.Tests.Fakes
{
    public sealed class FakeApiToken
    {
        private JwtOptions _jwtOptions;

        public FakeApiToken(JwtOptions jwtOptions)
        {
            _jwtOptions = jwtOptions;

            var random = new Random();

            TenantId = random.Next(1, int.MaxValue);
            UserId = random.Next(1, int.MaxValue);
        }

        public long TenantId { get; set; }
        public long UserId { get; set; }

        public override string ToString()
        {
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    key: new SymmetricSecurityKey(
                        key: Encoding.ASCII.GetBytes(_jwtOptions.Secret)
                    ),
                    algorithm: SecurityAlgorithms.HmacSha256
                ),
                claims: new[]
                {
                    new Claim("TenantId", TenantId.ToString()),
                    new Claim("UserId", UserId.ToString())
                }
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
