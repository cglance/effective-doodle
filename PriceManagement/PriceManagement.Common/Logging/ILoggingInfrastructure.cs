using System;

namespace PriceManagement.Common.Logging
{
    public interface ILoggingInfrastructure
    {
        ILogger GetLogger(params string[] name);

        ILogger GetLogger(Type type, params string[] qualifiers);
    }
}
