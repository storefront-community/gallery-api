namespace Storefront.Gallery.API.Models.IntegrationModel.FileStorage.AmazonS3
{
    public sealed class AmazonS3Options
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
    }
}
