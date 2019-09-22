using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PandaBank.SharedService.DataAccess
{ 
    public class SqlLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new QueryLogger();
        }

        public void Dispose()
        {
        }

        private class QueryLogger : ILogger
        {
            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception exception,
                Func<TState, Exception, string> formatter)
            {
                if (eventId.Id == 20101 || exception != null)
                {
                    Debug.WriteLine(" #!*#######*!# start #!*#######*!# ");
                    Debug.WriteLine("   ");
                    Debug.WriteLine(formatter(state, exception));
                    Debug.WriteLine("   ");
                    Debug.WriteLine(" #!*#######*!# end #!*#######*!# ");
                }
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}
