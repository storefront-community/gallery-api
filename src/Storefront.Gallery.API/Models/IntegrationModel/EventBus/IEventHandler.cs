using System.Threading.Tasks;

namespace Storefront.Gallery.API.Models.IntegrationModel.EventBus
{
    public interface IEventHandler
    {
        Task Handle(string message);
    }
}
