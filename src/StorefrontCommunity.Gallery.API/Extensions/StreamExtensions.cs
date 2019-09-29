using System;
using System.IO;
using System.Security.Cryptography;

namespace StorefrontCommunity.Gallery.API.Extensions
{
    public static class StreamExtensions
    {
        public static string Checksum(this Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
