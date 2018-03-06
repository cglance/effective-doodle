using System;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using PriceManagement.Common.Database;
using PriceManagement.Common.Patterns;

namespace PriceManagement.Common.Transaction
{
    public sealed class LocalTransaction : IDisposable
    {
        private static readonly AmbientStack<LocalTransaction> _ambient = new AmbientStack<LocalTransaction>();

        public static LocalTransaction Current => _ambient.TryPeek(out LocalTransaction tx)
            ? (tx._txScopeOption == TransactionScopeOption.Suppress ? null : tx)
            : null;

        private readonly TransactionScopeOption _txScopeOption;

        private readonly LocalTransaction _root;

        private IDbTransaction _dbTransaction;

        private IDatasource _trackedDatasource;

        private DeferredDisposingDatasourceConnection _connection;

        public LocalTransaction(TransactionScopeOption option = TransactionScopeOption.Required)
        {
            _txScopeOption = option;

            // if required, we'll just delegate to our parent transaction if it exists
            if (option == TransactionScopeOption.Required)
            {
                _root = Current?._root ?? this;
            }
            else
            {
                _root = this;
            }

            _ambient.Push(this);
        }

        public IDbTransaction DbTransaction => _root._dbTransaction;

        //TODO: this is klugy
        public async Task<IDatasourceConnection> BeginOrJoinTransactionAsync(IDatasource datasource)
        {
            if (_txScopeOption == TransactionScopeOption.Suppress)
            {
                return await datasource.GetConnectionAsync();
            }

            if (_root._trackedDatasource != null && _root._trackedDatasource != datasource)
            {
                // we're tracking a datasource, but not the one that we're trying to start a transaction on... we're not down with XA, yo
                throw new InvalidOperationException("This transaction is tracking a different datasource");
            }

            if (_root._dbTransaction == null)
            {
                // we don't have a db transaction yet, so open the connection and fire one up
                if (_root._trackedDatasource == null)
                {
                    _root._trackedDatasource = datasource;
                    _root._connection = _root._connection ?? new DeferredDisposingDatasourceConnection(await datasource.GetConnectionAsync());
                }

                _root._dbTransaction = _root._connection.DbConnection.BeginTransaction();
            }

            return _root._connection;
        }

        public void Complete()
        {
            if (_dbTransaction != null)
            {
                _dbTransaction.Commit();
                _dbTransaction = null;
            }
        }

        public void Dispose()
        {
            if (!ReferenceEquals(Current, this))
            {
                throw new InvalidOperationException("You cannot dispose a scope that is not the current ambient scope");
            }

            _ambient.Pop();

            if (_dbTransaction != null)
            {
                _dbTransaction.Rollback();
                _dbTransaction = null;
            }

            if (_connection != null)
            {
                _connection.Delegate.Dispose();
                _connection = null;
                _trackedDatasource = null;
            }
        }
    }
}
