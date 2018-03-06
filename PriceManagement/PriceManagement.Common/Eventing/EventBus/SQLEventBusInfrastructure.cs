using System.Data;
using System.Threading.Tasks;
using PriceManagement.Common.Database;

namespace PriceManagement.Common.Eventing.EventBus
{
    public class SQLEventBusInfrastructure : IEventBusInfrastructure
    {
        private readonly IDatasource _datasource;

        public SQLEventBusInfrastructure(IDatasource datasource)
        {
            _datasource = datasource;
        }

        public async Task EnqueueAsync(Message message)
        {
            const string sql = "insert into MessageQueue(Type, PublishedOnUTC, Payload) values(@Type, @PublishedOnUTC, @Payload)";

            using (IDatasourceConnection conn = await _datasource.GetConnectionAsync())
            {
                await conn.ExecuteNonQueryAsync(sql, new { message.Type, message.PublishedOnUTC, message.Payload });
            }
        }

        public async Task<(bool, Message)> TryDequeueAsync()
        {
            const string sql = @"
delete
  from MessageQueue
  output deleted.Type, deleted.Payload, deleted.PublishedOnUTC
  where Id = (select top 1 Id
                from MessageQueue with (readpast)
                order by PublishedOnUTC)";

            using (IDatasourceConnection connection = await _datasource.GetConnectionAsync())
            using (AsyncDbDataReader<IDataReader> reader = await connection.ExecuteReaderAsync(sql))
            {
                if (await reader.ReadAsync())
                {
                    var ordinalOf = new
                    {
                        Type = reader.DbReader.GetOrdinal("Type"),
                        Payload = reader.DbReader.GetOrdinal("Payload"),
                        PublishedOnUTC = reader.DbReader.GetOrdinal("PublishedOnUTC")
                    };

                    var message = new Message
                    {
                        Payload = await reader.Current.GetStream(ordinalOf.Payload).ReadFullyAndDisposeAsync(),
                        PublishedOnUTC = reader.Current.GetDateTime(ordinalOf.PublishedOnUTC),
                        Type = reader.Current.GetString(ordinalOf.Type)
                    };

                    return (true, message);
                }

                return (false, null);
            }
        }
    }
}
