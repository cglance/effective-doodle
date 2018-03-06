using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PriceManagement.Common.Database
{
    public class SQLServerDatasource : IDatasource
    {
        private readonly string _connectionString;

        public SQLServerDatasource(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IDatasourceConnection> GetConnectionAsync()
        {
            var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            return new DatasourceConnection(conn);
        }
    }
}
