using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PriceManagement.Common.Database;
using PriceManagement.Common.Eventing;
using PriceManagement.Common.Eventing.EventBus;
using PriceManagement.Common.Logging;
using PriceManagement.Common.Transaction;

namespace PriceManagement.Common.Test
{
    [TestClass]
    //[Ignore]
    public class EventBusTest
    {
        private EventBus bus;

        [TestInitialize]
        public void Setup()
        {
            var sqlDatasource = new SQLServerDatasource("Server=.\\sql2014;Database=Pricing;Trusted_Connection=True;");
            var txAwareDatasource = new LocalTransactionAwareDatasource(sqlDatasource);

            var logging = new ConsoleLoggingInfrastructure();
            var busInfrastructure = new SQLEventBusInfrastructure(txAwareDatasource);

            bus = EventBus.Configure(busInfrastructure, logging)
                .WithPollingInterval(5)
                .Publishes<string>(e => new Message
                {
                    Payload = Encoding.ASCII.GetBytes(e.Payload),
                    PublishedOnUTC = e.PublishedUTC,
                    Type = "Hello.World.Message"
                })
                .Publishes<int>(e => new Message
                {
                    Payload = Encoding.ASCII.GetBytes(e.Payload.ToString()),
                    PublishedOnUTC = e.PublishedUTC,
                    Type = "Hello.World.Message.Int"
                })
                .Handles(m =>
                {
                    if (m.Type == "Hello.World.Message")
                    {
                        Console.WriteLine(
                            $"got a message: {m.PublishedOnUTC} - {Encoding.ASCII.GetString(m.Payload)}");
                        return true;
                    }

                    return false;
                })
                .Handles(m =>
                {
                    if (m.Type == "Hello.World.Message.Int")
                    {
                        Console.WriteLine(
                            $"got an int message: {m.PublishedOnUTC} - {Encoding.ASCII.GetString(m.Payload)}");
                        return true;
                    }

                    return false;
                })
                .Start();
        }

        [TestCleanup]
        public void Teardown()
        {
            bus.Stop();
            bus = null;
        }

        [TestMethod]
        public async Task TestEventBusString()
        {

            await bus.PublishAsync(Event.Create("hello world"));

            Thread.Sleep(10000);
        }

        [TestMethod]
        public async Task TestEventBusInt()
        {
            await bus.PublishAsync(Event.Create(49));

            Thread.Sleep(10000);
        }

        [TestMethod]
        public async Task TestTransactionalPublishString_Rollback()
        {
            using (new LocalTransaction())
            {
                await bus.PublishAsync(Event.Create("This should rollback"));
            }
        }

        [TestMethod]
        public async Task TestTransactionalPublishString_Commit()
        {
            using (var tx = new LocalTransaction())
            {
                await bus.PublishAsync(Event.Create("This should be committed!"));
                tx.Complete();
            }
        }

        [TestMethod]
        public async Task TestPerformance_Commit()
        {
            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                using (var tx = new LocalTransaction())
                {
                    await bus.PublishAsync(Event.Create("hello world"));
                    tx.Complete();
                }
            }
            Console.WriteLine($"Published 1000 messages in {watch.Elapsed}");
        }

        [TestMethod]
        public async Task TestPerformance_SingleCommit()
        {
            Stopwatch watch = Stopwatch.StartNew();
            using (var tx = new LocalTransaction())
            {
                for (int i = 0; i < 1000; i++)
                {
                    await bus.PublishAsync(Event.Create("hello world"));
                }
                tx.Complete();
            }
            Console.WriteLine($"Published 1000 messages in {watch.Elapsed}");
        }

        [TestMethod]
        public async Task TestPerformance_Rollback()
        {
            Stopwatch watch = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                using (new LocalTransaction())
                {
                    await bus.PublishAsync(Event.Create("hello world"));
                }
            }
            Console.WriteLine($"Published 1000 messages in {watch.Elapsed}");
        }

        [TestMethod]
        public void RunFor1Minute()
        {
            Thread.Sleep(60000);
        }
    }
}
