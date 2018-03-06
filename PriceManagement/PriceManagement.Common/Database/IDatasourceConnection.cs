using System;
using System.Data;

namespace PriceManagement.Common.Database
{
    public interface IDatasourceConnection : IDisposable
    {
        IDbConnection DbConnection { get; }
    }
}