using System;

namespace PriceManagement.Common.Logging
{
    public interface ILogger
    {
        bool IsTraceEnabled { get; }
        void Trace(string message);
        void Trace(string message, Exception e);

        bool IsDebugEnabled { get; }
        void Debug(string message);
        void Debug(string message, Exception e);

        bool IsInfoEnabled { get; }
        void Info(string message);
        void Info(string message, Exception e);

        bool IsWarnEnabled { get; }
        void Warn(string message);
        void Warn(string message, Exception e);

        bool IsErrorEnabled { get; }
        void Error(string message);
        void Error(string message, Exception e);

        bool IsFatalEnabled { get; }
        void Fatal(string message);
        void Fatal(string message, Exception e);

        bool IsEnabled(Severity severity);
        void Log(Severity severity, string message);
        void Log(Severity severity, string message, Exception e);
    }
}
