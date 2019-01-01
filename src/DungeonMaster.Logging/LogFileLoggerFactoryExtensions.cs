using DungeonMaster;
using System;

namespace Microsoft.Extensions.Logging
{
    public static class LogFileLoggerFactoryExtensions
    {
        public static ILoggerFactory AddLogFile(this ILoggerFactory factory, Action<LogFileOptions> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            var options = new LogFileOptions();
            builder(options);
            options.Validate();
            var provider = new LogFileLoggerProvider(options);
            factory.AddProvider(provider);
            return factory;
        }
    }
}
