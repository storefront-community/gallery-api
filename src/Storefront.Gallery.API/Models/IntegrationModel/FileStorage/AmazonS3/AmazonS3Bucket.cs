using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace Storefront.Gallery.API.Models.IntegrationModel.FileStorage.AmazonS3
{
    [ExcludeFromCodeCoverage]
    public sealed class AmazonS3Bucket : IFileStorage
    {
        private readonly AmazonS3Options _options;
        private readonly AmazonS3Client _client;

        public AmazonS3Bucket(AmazonS3Options options)
        {
            _options = options;

            var region = RegionEndpoint.GetBySystemName(_options.Region);
            _client = new AmazonS3Client(_options.AccessKeyId, _options.SecretAccessKey, region);
        }

        public async Task Save(StoredFile file)
        {
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = file.Name.ToString(),
                ContentType = file.ContentType
            };

            request.InputStream = file.Stream;

            await _client.PutObjectAsync(request);
        }

        public async Task Delete(string fileName)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = fileName
            };

            await _client.DeleteObjectAsync(request);
        }

        public async Task<StoredFile> Read(string fileName)
        {
            var request = new GetObjectRequest
            {
                BucketName = _options.BucketName,
                Key = fileName
            };

            using (var response = await _client.GetObjectAsync(request))
            using (var responseStream = response.ResponseStream)
            {
                var memoryStream = new MemoryStream();

                await responseStream.CopyToAsync(memoryStream);

                return new StoredFile
                {
                    Name = fileName,
                    Stream = memoryStream,
                    ContentType = response.Headers.ContentType
                };
            }
        }
    }
}
