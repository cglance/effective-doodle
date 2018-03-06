using System;

namespace PriceManagement.Common.Eventing
{
    public abstract class Event
    {
        public static Event<T> Create<T>(T payload, DateTime? publishedUTC = null)
        {
            return new Event<T>(payload, publishedUTC);
        }

        public DateTime PublishedUTC { get; }

        public object Payload { get; }

        public Event(object payload, DateTime? publishedUTC = null)
        {
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            PublishedUTC = publishedUTC ?? DateTime.UtcNow;
        }
    }

    public class Event<T> : Event
    {
        public Event(T payload, DateTime? publishedUTC = null) : base(payload, publishedUTC)
        {
        }

        public new T Payload => (T) base.Payload;
    }
}
