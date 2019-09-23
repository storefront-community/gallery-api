using System.Threading.Tasks;

namespace Storefront.Gallery.API.Models.IntegrationModel.EventBus
{
    public abstract class EventHandler
    {
        public abstract Task Handle(string message);
    }
}
