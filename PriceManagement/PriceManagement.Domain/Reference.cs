using System;

namespace PriceManagement.Domain
{
    public class Reference<THeader, TId, TPayload> : Lazy<TPayload>
    {
        public string Type { get; }

        public TId Id { get; }

        public THeader Header { get; }

        public Reference(string type, TId id, THeader header, TPayload payload)
            : base(() => payload)
        {
            this.Type = type;
            this.Id = id;
            this.Header = header;
        }

        public Reference(string type, TId id, THeader header, Func<TPayload> payloadGenerator)
            : base(payloadGenerator)
        {
            this.Type = type;
            this.Id = id;
            this.Header = header;
        }
    }
}
