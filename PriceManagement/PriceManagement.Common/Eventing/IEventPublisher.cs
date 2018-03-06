using System.Threading.Tasks;

namespace PriceManagement.Common.Eventing
{
    public interface IEventPublisher
    {
        Task PublishAsync(Event @event);
    }
}