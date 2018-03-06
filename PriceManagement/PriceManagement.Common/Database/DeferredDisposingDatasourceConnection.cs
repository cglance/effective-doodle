using System.Data;

namespace PriceManagement.Common.Database
{
    public class DeferredDisposingDatasourceConnection : IDatasourceConnection
    {
        public IDatasourceConnection Delegate { get; }

        public IDbConnection DbConnection => Delegate.DbConnection;

        public DeferredDisposingDatasourceConnection(IDatasourceConnection @delegate)
        {
            Delegate = @delegate;
        }

        public void Dispose()
        {
            // don't do anything... someone else will do our disposing for us
        }
    }
}
