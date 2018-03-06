using System;
using System.Data;

namespace PriceManagement.Common.Database
{
    public class DatasourceConnection : IDatasourceConnection
    {
        public IDbConnection DbConnection { get; private set; }

        public DatasourceConnection(IDbConnection connection)
        {
            DbConnection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void Dispose()
        {
            if (DbConnection != null)
            {
                DbConnection.Dispose();
                DbConnection = null;
            }
        }
    }
}