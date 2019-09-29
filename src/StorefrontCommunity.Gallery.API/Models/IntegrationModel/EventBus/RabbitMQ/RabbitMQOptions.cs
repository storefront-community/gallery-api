namespace StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus.RabbitMQ
{
    public sealed class RabbitMQOptions
    {
        public string Host { get; set; }
        public string Exchange { get; set; }
        public string Queue { get; set; }
    }
}
