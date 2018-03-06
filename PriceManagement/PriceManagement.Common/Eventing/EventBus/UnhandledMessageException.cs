using System;

namespace PriceManagement.Common.Eventing.EventBus
{
    public class UnhandledMessageException : Exception
    {
        public Message UnhandledMessage { get; }

        public UnhandledMessageException(Message message)
            : base("No message handler accepted this message")
        {
            UnhandledMessage = message;
        }
    }
}
