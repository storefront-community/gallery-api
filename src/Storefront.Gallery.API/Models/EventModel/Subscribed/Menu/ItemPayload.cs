namespace Storefront.Gallery.API.Models.EventModel.Subscribed.Menu
{
    public sealed class ItemPayload
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
