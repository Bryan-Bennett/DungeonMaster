using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using System;

namespace DungeonMaster
{
    internal class LogFileLogger : ILogger
    {
        private LogFileOptions Options { get; }

        private LogFileProcessor Processor { get; }

        private string Category { get; }

        public LogFileLogger(string category, LogFileOptions options, LogFileProcessor processor)
        {
            Category = category ?? throw new ArgumentNullException(nameof(category));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (logLevel >= Options.MinimumLogLevel && logLevel != LogLevel.None);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (formatter == null)
            {
                return;
            }
            Processor.Log(Category, logLevel, eventId, state, exception, formatter);
        }
    }
}
