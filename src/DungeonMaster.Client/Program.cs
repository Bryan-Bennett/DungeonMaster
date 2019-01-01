using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using Unity;

namespace DungeonMaster
{
    partial class Program : IDisposable
    {
        public bool Running { get; private set; }

        private UnityContainer Container { get; } = new UnityContainer();

        private Program()
        {
        }

        public void Run()
        {
            if (Running)
            {
                throw new InvalidOperationException("The program is already running!");
            }
            Running = true;
            Configure(Container);
            //TODO
        }

        public void Dispose()
        {
            Container.Dispose();
        }

        private void Configure(IUnityContainer container)
        {
            var loggers = container.AddLogging();
            loggers.AddLogFile(x =>
            {
                x.FilePath = @"logs\log.txt";
#if DEBUG
                x.MinimumLogLevel = LogLevel.Debug;
#endif
            });

            AddFlightRecord(loggers);
        }

        [Conditional("DEBUG")]
        private void AddFlightRecord(ILoggerFactory loggers)
        {
            loggers.AddLogFile(x =>
             {
                 x.FilePath = @"logs\flightrecord.txt";
                 x.IsFlightRecord = true;
             });
        }
    }
}
