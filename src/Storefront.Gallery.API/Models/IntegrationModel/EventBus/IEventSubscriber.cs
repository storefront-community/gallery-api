using System.Threading.Tasks;

namespace Storefront.Gallery.API.Models.IntegrationModel.EventBus
{
    public interface IEventSubscriber
    {
        Task Handle(string message);
    }
}
