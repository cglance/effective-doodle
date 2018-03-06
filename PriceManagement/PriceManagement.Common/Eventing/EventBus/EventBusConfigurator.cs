using System;
using System.Collections.Generic;
using PriceManagement.Common.Logging;
using PriceManagement.Common.Patterns;

namespace PriceManagement.Common.Eventing.EventBus
{
    public class EventBusConfigurator
    {
        public const int DefaultPollingIntervalSeconds = 30;

        private readonly IEventBusInfrastructure _infrastructure;

        private readonly ILoggingInfrastructure _logging;

        private readonly ChainOfResponsibility<Message> _handlers = new ChainOfResponsibility<Message>();

        private readonly IDictionary<Type, Func<Event, Message>> _publishers = new Dictionary<Type, Func<Event, Message>>();
        
        private readonly ChainOfResponsibility<(Message, Exception, EventBus)> _errorHandlers = new ChainOfResponsibility<(Message, Exception, EventBus)>();

        private int _pollingIntervalInSeconds = DefaultPollingIntervalSeconds;

        public EventBusConfigurator(IEventBusInfrastructure infrastructure, ILoggingInfrastructure logging)
        {
            _infrastructure = infrastructure ?? throw new ArgumentNullException(nameof(infrastructure));
            _logging = logging ?? throw new ArgumentNullException(nameof(logging));
        }

        public EventBusConfigurator Handles(Func<Message, bool> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handlers.AddHandler(handler);
            return this;
        }

        public EventBusConfigurator Publishes<T>(Func<Event<T>, Message> publisher)
        {
            if(publisher == null) throw new ArgumentNullException(nameof(publisher));
            _publishers.Add(typeof(T), e => publisher((Event<T>) e));
            return this;
        }

        public EventBusConfigurator OnError(Func<Message, Exception, EventBus, bool> errorHandler)
        {
            if(errorHandler == null) throw new ArgumentNullException(nameof(errorHandler));
            _errorHandlers.AddHandler(t => errorHandler(t.Item1, t.Item2, t.Item3));
            return this;
        }

        public EventBusConfigurator WithPollingInterval(int pollingIntervalInSeconds)
        {
            if(pollingIntervalInSeconds <= 0) throw new ArgumentException("Polling interval must be positive");
            _pollingIntervalInSeconds = pollingIntervalInSeconds;
            return this;
        }

        public EventBus Start()
        {
            return new EventBus(_infrastructure, _handlers, _publishers, _errorHandlers, _pollingIntervalInSeconds, _logging);
        }
    }
}
