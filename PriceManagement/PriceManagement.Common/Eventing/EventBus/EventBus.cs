using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PriceManagement.Common.Logging;
using PriceManagement.Common.Patterns;
using PriceManagement.Common.Transaction;

namespace PriceManagement.Common.Eventing.EventBus
{
    public class EventBus : IDisposable, IEventPublisher
    {
        public static EventBusConfigurator Configure(IEventBusInfrastructure infrastructure, ILoggingInfrastructure logging)
        {
            return new EventBusConfigurator(infrastructure, logging);
        }

        private readonly IEventBusInfrastructure _infrastructure;

        private readonly ChainOfResponsibility<Message> _handlers;

        private readonly IDictionary<Type, Func<Event, Message>> _publishers;

        private readonly ChainOfResponsibility<(Message, Exception, EventBus)> _errorHandlers;

        private readonly int _pollingIntervalInSeconds;

        private readonly CancellationTokenSource _daemonCancellationToken = new CancellationTokenSource();

        private readonly ILogger _logger;

        internal EventBus(IEventBusInfrastructure infrastructure, ChainOfResponsibility<Message> handlers, IDictionary<Type, Func<Event, Message>> publishers,
            ChainOfResponsibility<(Message, Exception, EventBus)> errorHandlers, int pollingIntervalInSeconds, ILoggingInfrastructure logging)
        {
            _logger = logging.GetLogger(typeof(EventBus));

            _infrastructure = infrastructure;
            _handlers = handlers;
            _publishers = publishers;
            _errorHandlers = errorHandlers;
            _pollingIntervalInSeconds = pollingIntervalInSeconds;

            // fire up the daemon thread
            var daemonThread = new Thread(Daemon) {IsBackground = true};
            daemonThread.Start();
        }

        public async Task PublishAsync(Event @event)
        {
            if(@event == null) throw new ArgumentNullException(nameof(@event));

            if (!_publishers.TryGetValue(@event.Payload.GetType(), out Func<Event, Message> serializer))
            {
                throw new Exception("No Publisher found");
            }

            Message message = serializer(@event);
            await _infrastructure.EnqueueAsync(message);
        }

        public void Stop()
        {
            _daemonCancellationToken.Cancel();
        }

        public void Dispose()
        {
            Stop();
        }

        private void Daemon()
        {
            while (!_daemonCancellationToken.IsCancellationRequested)
            {
                bool success = true;
                while (success && !_daemonCancellationToken.IsCancellationRequested)
                {
                    var task = TryHandleMessageAsync();
                    task.Wait(_daemonCancellationToken.Token);
                    success = task.Result;
                }
                Thread.Sleep(_pollingIntervalInSeconds * 1000);
            }
        }

        private async Task<bool> TryHandleMessageAsync()
        {
            try
            {
                _logger.Trace("About to try to handle a message");
                using (var tx = new LocalTransaction())
                {
                    (bool success, Message message) = await _infrastructure.TryDequeueAsync();
                    if (success)
                    {
                        try
                        {
                            if (!_handlers.Execute(message))
                            {
                                _logger.Warn($"No handler found for message {message}");
                                throw new UnhandledMessageException(message);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.Error($"An exception was encountered while processing a message: {message}", e);

                            try
                            {
                                _errorHandlers.Execute((message, e, this));
                            }
                            catch (Exception errorHandlerException)
                            {
                                _logger.Error("Additionally, the error handler throw an exception",
                                    errorHandlerException);
                            }
                        }

                        tx.Complete();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                //TODO: what should happen here? if we rollback this will most likely be an infinite loop
                _logger.Error("Unhandled exception encountered while trying to process a message", e);
            }

            return false;
        }
    }
}
