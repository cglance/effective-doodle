using System;

namespace PriceManagement.Common.Logging
{
    public class ConsoleLoggingInfrastructure : ILoggingInfrastructure
    {
        public ILogger GetLogger(params string[] name)
        {
            return new ConsoleLogger(string.Join(".", name));
        }

        public ILogger GetLogger(Type type, params string[] qualifiers)
        {
            string name = type.FullName;
            if (qualifiers != null && qualifiers.Length > 0)
            {
                name += "." + string.Join(".", qualifiers);
            }

            return new ConsoleLogger(name);
        }
    }

    public class ConsoleLogger : ILogger
    {
        private readonly string _name;

        public ConsoleLogger(string name)
        {
            _name = name;
        }

        public bool IsTraceEnabled => IsEnabled(Severity.Trace);
        public void Trace(string message)
        {
            Trace(message, null);
        }

        public void Trace(string message, Exception e)
        {
            Log(Severity.Trace, message, e);
        }

        public bool IsDebugEnabled => IsEnabled(Severity.Debug);
        public void Debug(string message)
        {
            Debug(message, null);
        }

        public void Debug(string message, Exception e)
        {
            Log(Severity.Debug, message, e);
        }

        public bool IsInfoEnabled => IsEnabled(Severity.Info);
        public void Info(string message)
        {
            Info(message, null);
        }

        public void Info(string message, Exception e)
        {
            Log(Severity.Info, message, e);
        }

        public bool IsWarnEnabled => IsEnabled(Severity.Warn);
        public void Warn(string message)
        {
            Warn(message, null);
        }

        public void Warn(string message, Exception e)
        {
            Log(Severity.Warn, message, e);
        }

        public bool IsErrorEnabled => IsEnabled(Severity.Error);
        public void Error(string message)
        {
            Error(message, null);
        }

        public void Error(string message, Exception e)
        {
            Log(Severity.Error, message, e);
        }

        public bool IsFatalEnabled => IsEnabled(Severity.Fatal);
        public void Fatal(string message)
        {
            Fatal(message, null);
        }

        public void Fatal(string message, Exception e)
        {
            Log(Severity.Fatal, message, e);
        }

        public bool IsEnabled(Severity severity)
        {
            return true;
        }

        public void Log(Severity severity, string message)
        {
            Log(severity, message, null);
        }

        public void Log(Severity severity, string message, Exception e)
        {
            Console.WriteLine($"[{DateTime.Now}] [{Enum.GetName(typeof(Severity), severity)}] [{_name}] {message}");
            if (e != null)
            {
                Console.WriteLine(e);
            }
        }
    }
}
