using System.Threading.Tasks;

namespace StorefrontCommunity.Gallery.API.Models.IntegrationModel.EventBus
{
    public interface IEventSubscriber
    {
        Task Handle(string message);
    }
}
