using System;

namespace Storefront.Gallery.API.Models.IntegrationModel.EventBus
{
    public interface IEvent
    {
        Guid Id { get; }
        DateTime CreatedAt { get; }
        string Name { get; }
        object Payload { get; }
    }
}
