using System.Threading.Tasks;
using Newtonsoft.Json;
using Storefront.Gallery.API.Models.IntegrationModel.EventBus;

namespace Storefront.Gallery.API.Models.EventModel.Subscribed.Menu
{
    public sealed class ItemGroupDeletedEvent : IEventHandler
    {
        public Task Handle(string message)
        {
            var json = JsonConvert.DeserializeObject<Event<ItemGroupPayload>>(message);

            // TODO

            return Task.CompletedTask;
        }
    }
}
