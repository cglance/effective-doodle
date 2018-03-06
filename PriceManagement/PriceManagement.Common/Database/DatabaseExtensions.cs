using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using PriceManagement.Common.DataTypes;
using PriceManagement.Common.Transaction;

namespace PriceManagement.Common.Database
{
    public static class DatabaseExtensions
    {
        //
        // Connection
        //

        public static async Task OpenAsync(this IDbConnection connection, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connection is DbConnection)
            {
                await ((DbConnection) connection).OpenAsync(cancellationToken);
            }
            else
            {
                connection.Open();
            }
        }

        public static IDbCommand CreateCommand(this IDbConnection conn, string commandText, CommandType commandType = CommandType.Text, int commandTimeout = -1,
            IDbTransaction transaction = null)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = commandText;
            if (commandTimeout >= 0)
            {
                cmd.CommandTimeout = commandTimeout;
            }

            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            return cmd;
        }

        //
        // Command
        //

        public static async Task<int> ExecuteNonQueryAsync(this IDbCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (command is DbCommand)
            {
                return await ((DbCommand) command).ExecuteNonQueryAsync(cancellationToken);
            }
            
            return command.ExecuteNonQuery();
        }

        public static async Task<IDataReader> ExecuteReaderAsync(this IDbCommand command, CommandBehavior behavior = CommandBehavior.Default, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (command is DbCommand)
            {
                return await ((DbCommand) command).ExecuteReaderAsync(behavior, cancellationToken);
            }

            return command.ExecuteReader(behavior);
        }

        public static IDbDataParameter AddParameterWithValue(this IDbCommand command, string parameterName, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value;
            command.Parameters.Add(parameter);
            return parameter;
        }

        public static void AddParametersWithValues(this IDbCommand command, object parameters)
        {
            foreach (PropertyInfo prop in parameters.GetType().GetProperties())
            {
                command.AddParameterWithValue(prop.Name, prop.GetValue(parameters));
            }
        }

        //
        // Reader
        //

        public static async Task<bool> ReadAsync(this IDataReader reader, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (reader is DbDataReader)
            {
                return await ((DbDataReader) reader).ReadAsync(cancellationToken);
            }

            return reader.Read();
        }

        public static string GetStringNullable(this IDataReader reader, int ordinal)
        {
            var result = reader[ordinal];
            return result == DBNull.Value ? null : (string) result;
        }

        public static Int32? GetInt32Nullable(this IDataReader reader, int ordinal)
        {
            var result = reader[ordinal];
            return result == DBNull.Value ? (Int32?) null : (Int32) result;
        }

        public static Int64? GetInt64Nullable(this IDataReader reader, int ordinal)
        {
            var result = reader[ordinal];
            return result == DBNull.Value ? (Int64?)null : (Int64)result;
        }

        public static DateTime? GetDateTimeNullable(this IDataReader reader, int ordinal)
        {
            var result = reader[ordinal];
            return result == DBNull.Value ? (DateTime?) null : (DateTime) result;
        }

        public static Boolean? GetBooleanNullable(this IDataReader reader, int ordinal)
        {
            var result = reader[ordinal];
            return result == DBNull.Value ? (Boolean?)null : (Boolean)result;
        }

        public static Date GetDate(this IDataReader reader, int ordinal)
        {
            var datetime = reader.GetDateTime(ordinal);
            return Date.FromDateTime(datetime);
        }

        public static Date GetDateNullable(this IDataReader reader, int ordinal)
        {
            var datetime = reader.GetDateTimeNullable(ordinal);
            return datetime == null ? null : Date.FromDateTime(datetime.Value);
        }

        public static DateRange GetDateRange(this IDataReader reader, int beginDateOrdinal, int endDateOrdinal)
        {
            return new DateRange(reader.GetDateNullable(beginDateOrdinal), reader.GetDateNullable(endDateOrdinal));
        }

        public static Stream GetStream(this IDataReader reader, int ordinal)
        {
            if (reader is DbDataReader)
            {
                return ((DbDataReader) reader).GetStream(ordinal);
            }

            var stream = new MemoryStream();
            byte[] buffer = new byte[4096];
            for(long offset = 0, count = 0; (count = reader.GetBytes(ordinal, offset, buffer, 0, buffer.Length)) > 0; offset += count)
            {
                stream.Write(buffer, 0, (int) count);
            }

            return stream;
        }

        public static async Task<byte[]> ReadFullyAsync(this Stream stream)
        {
            bool close = false;
            if (!(stream is MemoryStream memoryStream))
            {
                memoryStream = new MemoryStream { Capacity = (int)stream.Length };
                close = true;
                await stream.CopyToAsync(memoryStream);
            }

            byte[] bytes = memoryStream.ToArray();
            if(close) memoryStream.Dispose();;

            return bytes;
        }

        public static async Task<byte[]> ReadFullyAndDisposeAsync(this Stream stream)
        {
            byte[] bytes = await stream.ReadFullyAsync();
            stream.Dispose();
            return bytes;
        }
        
        //
        // Higher-order utilities
        //

        public static async Task<int> ExecuteNonQueryAsync(this IDatasourceConnection connection, string commandText,
            object parameters = null, bool joinTransaction = true,
            CommandType commandType = CommandType.Text, int commandTimeout = -1,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IDbTransaction tx = null;
            if (joinTransaction)
            {
                tx = LocalTransaction.Current?.DbTransaction;
                if(tx != null && !ReferenceEquals(tx.Connection, connection.DbConnection))
                {
                    tx = null;
                }
            }

            using (IDbCommand cmd = connection.DbConnection.CreateCommand(commandText, commandType, commandTimeout, tx))
            {
                if (parameters != null)
                {
                    cmd.AddParametersWithValues(parameters);
                }

                return await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public static async Task<AsyncDbDataReader<IDataReader>> ExecuteReaderAsync(this IDatasourceConnection connection, string commandText,
            object parameters = null, bool joinTransaction = true,
            CommandType commandType = CommandType.Text, int commandTimeout = -1,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            IDbTransaction tx = null;
            if (joinTransaction)
            {
                tx = LocalTransaction.Current?.DbTransaction;
                if (tx != null && !ReferenceEquals(tx.Connection, connection.DbConnection))
                {
                    tx = null;
                }
            }

            IDbCommand cmd = connection.DbConnection.CreateCommand(commandText, commandType, commandTimeout, tx);
            if (parameters != null)
            {
                cmd.AddParametersWithValues(parameters);
            }

            IDataReader reader = await cmd.ExecuteReaderAsync(behavior, cancellationToken);
            return new AsyncDbDataReader<IDataReader>(reader, cmd, null, x => x);
        }

        public static async Task<AsyncDbDataReader<T>> ExecuteReaderAsync<T>(this IDatasourceConnection connection, string commandText,
            object parameters = null, bool joinTransaction = true,
            CommandType commandType = CommandType.Text, int commandTimeout = -1,
            CommandBehavior behavior = CommandBehavior.Default,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : new()
        {
            IDbTransaction tx = null;
            if (joinTransaction)
            {
                tx = LocalTransaction.Current?.DbTransaction;
                if (tx != null && !ReferenceEquals(tx.Connection, connection.DbConnection))
                {
                    tx = null;
                }
            }

            IDbCommand cmd = null;
            IDataReader reader = null;
            try
            {
                cmd = connection.DbConnection.CreateCommand(commandText, commandType, commandTimeout, tx);
                if (parameters != null)
                {
                    cmd.AddParametersWithValues(parameters);
                }

                reader = await cmd.ExecuteReaderAsync(behavior, cancellationToken);

                return new AsyncDbDataReader<T>(reader, cmd, null, ConvertByColumnNames<T>);
            }
            catch (Exception)
            {
                reader?.Dispose();
                cmd?.Dispose();
                throw;
            }
        }

        //TODO: This will definitely benefit from some caching
        private static T ConvertByColumnNames<T>(IDataReader reader)
            where T : new()
        {
            if (typeof(T) == typeof(object))
            {
                // deal with dynamic
                IDictionary<string, object> result = new ExpandoObject();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    result.Add(reader.GetName(i), reader.GetValue(i));
                }

                return (dynamic) result;
            }
            else
            {
                T result = new T();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo property = typeof(T).GetProperty(reader.GetName(i));
                    property.SetValue(result, reader.GetValue(i));
                }

                return result;
            }
        }
    }
}
