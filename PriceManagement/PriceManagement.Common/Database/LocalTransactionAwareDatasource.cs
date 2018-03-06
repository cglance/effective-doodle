using System.Threading.Tasks;
using PriceManagement.Common.Transaction;

namespace PriceManagement.Common.Database
{
    public class LocalTransactionAwareDatasource : IDatasource
    {
        private readonly IDatasource _underlying;

        public LocalTransactionAwareDatasource(IDatasource underlying)
        {
            _underlying = underlying;
        }

        public async Task<IDatasourceConnection> GetConnectionAsync()
        {
            LocalTransaction tx = LocalTransaction.Current;
            if (tx != null)
            {
                return await tx.BeginOrJoinTransactionAsync(_underlying);
            }

            return await _underlying.GetConnectionAsync();
        }
    }
}
