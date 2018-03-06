using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceManagement.Common.Eventing.EventBus
{
    public class InMemoryEventBusInfrastructure : IEventBusInfrastructure
    {
        private readonly Queue<Message> _q = new Queue<Message>();

        public Task EnqueueAsync(Message message)
        {
            _q.Enqueue(message);
            return Task.CompletedTask;
        }

        public Task<(bool, Message)> TryDequeueAsync()
        {
            return Task.FromResult(_q.Count > 0 ? (true, _q.Dequeue()) : (false, null));
        }
    }
}
