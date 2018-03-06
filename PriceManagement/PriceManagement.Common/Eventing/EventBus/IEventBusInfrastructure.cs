using System.Threading.Tasks;

namespace PriceManagement.Common.Eventing.EventBus
{
    public interface IEventBusInfrastructure
    {
        Task EnqueueAsync(Message message);

        Task<(bool, Message)> TryDequeueAsync();
    }
}
