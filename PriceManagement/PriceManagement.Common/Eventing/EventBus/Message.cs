using System;

namespace PriceManagement.Common.Eventing.EventBus
{
    public class Message
    {
        public string Type { get; set; }

        public DateTime PublishedOnUTC { get; set; }

        public byte[] Payload { get; set; }

        public override string ToString()
        {
            return $"( {nameof(Type)}: {Type}, {nameof(PublishedOnUTC)}: {PublishedOnUTC}, {nameof(Payload)}: {Payload} )";
        }
    }
}