using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace PriceManagement.Common.Database
{
    public class AsyncDbDataReader<T> : IDisposable
    {
        private IDbCommand _command;

        private IDbConnection _connection;

        private readonly Func<IDataReader, T> _conversion;

        public AsyncDbDataReader(IDataReader dbReader, IDbCommand command, IDbConnection connection, Func<IDataReader, T> conversion)
        {
            DbReader = dbReader;
            _command = command;
            _connection = connection;
            _conversion = conversion;
        }

        public IDataReader DbReader { get; private set; }

        public T Current { get; private set; }
        
        public async Task<bool> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (await DbReader.ReadAsync(cancellationToken))
            {
                Current = _conversion(DbReader);
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            if (DbReader != null)
            {
                DbReader.Dispose();
                DbReader = null;
            }

            if (_command != null)
            {
                _command.Dispose();
                _command = null;
            }

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
