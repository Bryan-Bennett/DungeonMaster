using Microsoft.Extensions.Logging;
using System;

namespace DungeonMaster
{
    public class LogFileLoggerProvider : ILoggerProvider
    {
        private LogFileOptions Options { get; }

        private LogFileProcessor Processor { get; }

        public LogFileLoggerProvider(LogFileOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Processor = new LogFileProcessor(options);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new LogFileLogger(categoryName, Options, Processor);
        }

        public void Dispose()
        {
            Processor.Dispose();
        }
    }
}
